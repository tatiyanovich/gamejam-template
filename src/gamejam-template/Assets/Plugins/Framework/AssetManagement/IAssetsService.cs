using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.AssetManagement
{
	public interface IAssetsService
	{
		UniTask Initialize();

		void Dispose();
		void FullCleanUp();
		void CleanUpCategory(string category);
		void CleanAsset(string key);
		void CleanAsset(AssetReference assetReference);

		TAsset Load<TAsset>(string key) where TAsset : UnityEngine.Object;
		UniTask<TAsset> LoadAsync<TAsset>(string key, string category = "common", CancellationToken cancellationToken = default) where TAsset : UnityEngine.Object;
		UniTask<TAsset> LoadAsync<TAsset>(AssetReference assetReference, string category = "common", CancellationToken cancellationToken = default) where TAsset : UnityEngine.Object;
		UniTask<TComponent> LoadAsyncForComponent<TComponent>(string key, string category = "common", CancellationToken cancellationToken = default) where TComponent : Component;
		UniTask<TComponent> LoadAsyncForComponent<TComponent>(AssetReference assetReference, string category = "common", CancellationToken cancellationToken = default) where TComponent : Component;
		UniTask<TAsset[]> LoadAllAsync<TAsset>(List<string> keys) where TAsset : UnityEngine.Object;
		List<string> GetAssetsListByLabel<TAsset>(string label);
		List<string> GetAssetsListByLabel(string label, Type type = null);
		TAsset[] GetAssetsByLabel<TAsset>(string label) where TAsset : UnityEngine.Object;
		UniTask<TAsset[]> GetAssetsByLabelAsync<TAsset>(string label, string category = "common", CancellationToken cancellationToken = default) where TAsset : UnityEngine.Object;
		UniTask<List<string>> GetAssetsListByLabelAsync<TAsset>(string label);
		UniTask<List<string>> GetAssetsListByLabelAsync(string label, Type type = null, CancellationToken cancellationToken = default);
		GameObject LoadAssetFromResources(string path);
		TUnityObject LoadAssetFromResources<TUnityObject>(string path) where TUnityObject : UnityEngine.Object;
		TUnityObject[] LoadAssetsFromResources<TUnityObject>(string path) where TUnityObject : UnityEngine.Object;
	}
}
