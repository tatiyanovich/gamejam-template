using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Framework.AssetManagement
{
	public class AssetsService : IAssetsService, IDisposable
	{
		private readonly Dictionary<string, AsyncOperationHandle> _assetRequests = new();
		private readonly Dictionary<string, List<string>> _categories = new();

		public async UniTask Initialize()
		{
			await Addressables.InitializeAsync().ToUniTask();
		}

		public void Dispose()
		{
			FullCleanUp();
		}

		public void FullCleanUp()
		{
			foreach (KeyValuePair<string, AsyncOperationHandle> assetRequest in _assetRequests)
			{
				if (assetRequest.Value.IsDone == false)
					assetRequest.Value.WaitForCompletion();

				if (assetRequest.Value.IsValid())
					Addressables.Release(assetRequest.Value);
			}

			_categories.Clear();
			_assetRequests.Clear();
		}

		public void CleanUpCategory(string category)
		{
			int number = 0;

			if (_categories.TryGetValue(category, out List<string> keys))
			{
				foreach (string key in keys)
				{
					if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
					{
						if (handle.IsDone == false)
							handle.WaitForCompletion();

						if (handle.IsValid())
							Addressables.Release(handle);

						_assetRequests.Remove(key);
						number++;
					}
				}

				_categories.Remove(category);
			}

			Debug.Log($"Cleaned up {number} assets from category {category}");
		}

		public void CleanAsset(string key)
		{
			if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle))
			{
				Addressables.Release(handle);
				_assetRequests.Remove(key);

				foreach (KeyValuePair<string, List<string>> category in _categories)
				{
					category.Value.Remove(key);
				}

				Debug.Log($"Cleaned up asset with key {key}");
			}
		}

		public void CleanAsset(AssetReference assetReference)
		{
			CleanAsset(assetReference.AssetGUID);
		}

		public TAsset Load<TAsset>(string key)
			where TAsset : UnityEngine.Object
		{
			if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle) == false)
			{
				handle = Addressables.LoadAssetAsync<TAsset>(key);
				_assetRequests.Add(key, handle);
			}

			object result = handle.WaitForCompletion();

			return result as TAsset;
		}

		public async UniTask<TAsset> LoadAsync<TAsset>(string key, string category = "common", CancellationToken cancellationToken = default)
			where TAsset : UnityEngine.Object
		{
			if (_assetRequests.TryGetValue(key, out AsyncOperationHandle handle) == false)
			{
				handle = Addressables.LoadAssetAsync<TAsset>(key);
				_assetRequests.Add(key, handle);
			}

			await handle.ToUniTask(cancellationToken: cancellationToken);

			AddToCategory(category, key);

			return handle.Result as TAsset;
		}

		public async UniTask<TAsset> LoadAsync<TAsset>(
			AssetReference assetReference, string category = "common", CancellationToken cancellationToken = default)
			where TAsset : UnityEngine.Object
		{
			return await LoadAsync<TAsset>(assetReference.AssetGUID, category, cancellationToken);
		}

		public async UniTask<TComponent> LoadAsyncForComponent<TComponent>(
			string key, string category = "common", CancellationToken cancellationToken = default)
			where TComponent : Component
		{
			GameObject prefab = await LoadAsync<GameObject>(key, category, cancellationToken);

			return prefab.TryGetComponent(out TComponent component)
				? component
				: throw new Exception($"Failed to get component {typeof(TComponent)} from prefab {key}");
		}

		public async UniTask<TComponent> LoadAsyncForComponent<TComponent>(
			AssetReference assetReference, string category = "common", CancellationToken cancellationToken = default)
			where TComponent : Component
		{
			GameObject prefab = await LoadAsync<GameObject>(assetReference, category, cancellationToken);

			return prefab.TryGetComponent(out TComponent component)
				? component
				: throw new Exception($"Failed to get component {typeof(TComponent)} from prefab {assetReference.AssetGUID}");
		}

		public async UniTask<TAsset[]> LoadAllAsync<TAsset>(List<string> keys)
			where TAsset : UnityEngine.Object
		{
			List<UniTask<TAsset>> tasks = new(keys.Count);

			foreach (string key in keys)
			{
				tasks.Add(LoadAsync<TAsset>(key));
			}

			return await UniTask.WhenAll(tasks);
		}

		public List<string> GetAssetsListByLabel<TAsset>(string label)
		{
			return GetAssetsListByLabel(label, typeof(TAsset));
		}

		public List<string> GetAssetsListByLabel(string label, Type type = null)
		{
			AsyncOperationHandle<IList<IResourceLocation>> operationHandle = Addressables.LoadResourceLocationsAsync(label, type);

			IList<IResourceLocation> locations = operationHandle.WaitForCompletion();

			List<string> assetKeys = new(locations.Count);

			foreach (IResourceLocation location in locations)
			{
				assetKeys.Add(location.PrimaryKey);
			}

			Addressables.Release(operationHandle);
			return assetKeys;
		}

		public TAsset[] GetAssetsByLabel<TAsset>(string label)
			where TAsset : UnityEngine.Object
		{
			List<string> assetKeys = GetAssetsListByLabel(label);

			TAsset[] result = new TAsset[assetKeys.Count];

			for (int i = 0; i < assetKeys.Count; i++)
			{
				result[i] = Load<TAsset>(assetKeys[i]);
			}

			return result;
		}

		public async UniTask<TAsset[]> GetAssetsByLabelAsync<TAsset>(
			string label, string category = "common", CancellationToken cancellationToken = default)
			where TAsset : UnityEngine.Object
		{
			List<string> assetKeys = await GetAssetsListByLabelAsync(label, cancellationToken: cancellationToken);

			TAsset[] result = new TAsset[assetKeys.Count];

			for (int i = 0; i < assetKeys.Count; i++)
			{
				result[i] = await LoadAsync<TAsset>(assetKeys[i], category, cancellationToken);
			}

			return result;
		}

		public async UniTask<List<string>> GetAssetsListByLabelAsync<TAsset>(string label)
		{
			return await GetAssetsListByLabelAsync(label, typeof(TAsset));
		}

		public async UniTask<List<string>> GetAssetsListByLabelAsync(string label, Type type = null, CancellationToken cancellationToken = default)
		{
			AsyncOperationHandle<IList<IResourceLocation>> operationHandle = Addressables.LoadResourceLocationsAsync(label, type);

			try
			{
				IList<IResourceLocation> locations = await operationHandle.ToUniTask(cancellationToken: cancellationToken);

				List<string> assetKeys = new(locations.Count);

				foreach (IResourceLocation location in locations)
				{
					assetKeys.Add(location.PrimaryKey);
				}

				return assetKeys;
			}
			finally
			{
				if (operationHandle.IsValid())
					Addressables.Release(operationHandle);
			}
		}

		public GameObject LoadAssetFromResources(string path)
		{
			return Resources.Load<GameObject>(path);
		}

		public TUnityObject LoadAssetFromResources<TUnityObject>(string path)
			where TUnityObject : UnityEngine.Object
		{
			return Resources.Load<TUnityObject>(path);
		}

		public TUnityObject[] LoadAssetsFromResources<TUnityObject>(string path)
			where TUnityObject : UnityEngine.Object
		{
			return Resources.LoadAll<TUnityObject>(path);
		}

		private void AddToCategory(string category, string key)
		{
			if (_categories.TryGetValue(category, out List<string> keys) == false)
			{
				keys = new List<string>();
				_categories.Add(category, keys);
			}

			if (keys.Contains(key) == false)
				keys.Add(key);
		}
	}
}
