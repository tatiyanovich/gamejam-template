using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.AssetManagement;
using Framework.Instantiation;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Essentials.ViewManagement.Services
{
	public class ViewFactory : IViewFactory
	{
		private readonly IAssetsService _assetsService;
		private readonly IInstantiateService _instantiateService;
		private readonly IViewPool _viewPool;

		public ViewFactory(
			IAssetsService assetsService,
			IInstantiateService instantiateService,
			IViewPool viewPool)
		{
			_assetsService = assetsService;
			_instantiateService = instantiateService;
			_viewPool = viewPool;
		}

		public IUnityView CreateViewFromResourcesPath(string path, Vector3 at)
		{
			GameObject viewPrefab = _assetsService.LoadAssetFromResources<GameObject>(path);

			if (viewPrefab == null)
				throw new NullReferenceException($"View prefab by path {path} is null");

			GameObject view = _viewPool.Has(path)
				? _viewPool.Get(path).gameObject
				: _instantiateService.Instantiate(viewPrefab, at, Quaternion.identity);

			IUnityView unityView = view.GetComponent<IUnityView>();

			if (unityView == null)
				throw new NullReferenceException($"View prefab by path {path} does not implement {nameof(IUnityView)}");

			return unityView;
		}

		public IUnityView CreateViewFromPrefab(IUnityView prefab, Vector3 at)
		{
			string viewPath = prefab.gameObject.GetInstanceID().ToString();

			IUnityView unityView = _viewPool.Has(viewPath)
				? _viewPool.Get(viewPath)
				: _instantiateService
					.Instantiate(prefab.gameObject, at, Quaternion.identity)
					.GetComponent<IUnityView>();

			return unityView;
		}

		public async UniTask<IUnityView> CreateViewFromAssetReference(
			AssetReference assetReference, Vector3 at, CancellationToken cancellationToken = default)
		{
			GameObject view;
			string assetGuid = assetReference.AssetGUID;

			if (_viewPool.Has(assetGuid))
			{
				view = _viewPool.Get(assetGuid).gameObject;
			}
			else
			{
				GameObject viewPrefab = await _assetsService.LoadAsync<GameObject>(assetReference, cancellationToken: cancellationToken);

				if (viewPrefab == null)
					throw new NullReferenceException($"View prefab {assetReference} is null");

				view = _instantiateService.Instantiate(viewPrefab, at, Quaternion.identity);
			}

			IUnityView unityView = view.GetComponent<IUnityView>();

			if (unityView == null)
				throw new NullReferenceException($"View prefab does not implement {nameof(IUnityView)}");

			return unityView;
		}

		public async UniTask<IUnityView> CreateViewFromAddressableKey(
			string addressableKey, Vector3 at, CancellationToken cancellationToken = default)
		{
			GameObject view;

			if (_viewPool.Has(addressableKey))
			{
				view = _viewPool.Get(addressableKey).gameObject;
			}
			else
			{
				GameObject viewPrefab = await _assetsService.LoadAsync<GameObject>(addressableKey, cancellationToken: cancellationToken);

				if (viewPrefab == null)
					throw new NullReferenceException($"View prefab by path {addressableKey} is null");

				view = _instantiateService.Instantiate(viewPrefab, at, Quaternion.identity);
			}

			IUnityView unityView = view.GetComponent<IUnityView>();

			if (unityView == null)
				throw new NullReferenceException($"View prefab does not implement {nameof(IUnityView)}");

			return unityView;
		}
	}
}
