using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Framework.AssetManagement;
using Framework.Essentials.ViewManagement.Services;
using Framework.Instantiation;
using UnityEngine;
using Zenject;

namespace Code.Infrastructure.EntityComponentSystem.Factories
{
	public class EntityViewFactory : IEntityViewFactory
	{
		private readonly Vector3 _farAway = new(-999, 999, 0);

		private readonly IAssetsService _assetsService;
		private readonly IInstantiateService _instantiateService;
		private readonly IViewPool _viewPool;

		private Transform _generalParent;
		private readonly Dictionary<string, Transform> _concreteParents = new();

		public EntityViewFactory(
			IAssetsService assetsService,
			IInstantiateService instantiateService,
			IViewPool viewPool)
		{
			_assetsService = assetsService;
			_instantiateService = instantiateService;
			_viewPool = viewPool;
		}

		public string GetViewPath(GameEntity entity)
		{
			if (entity.hasViewPath)
				return entity.ViewPath;

			if (entity.hasViewPrefab)
				return entity.ViewPrefab.GetInstanceID().ToString();

			if (entity.hasViewAssetReference)
				return entity.ViewAssetReference.AssetGUID;

			if (entity.hasViewAddressableKey)
				return entity.ViewAddressableKey;

			return null;
		}

		public EntityView CreateViewFromResourcesPath(GameEntity entity)
		{
			Vector3 position = GetSpawnPosition(entity);
			Quaternion rotation = GetSpawnRotation(entity);

			EntityView prefab = _assetsService.LoadAssetFromResources<EntityView>(entity.ViewPath);
			if (prefab == null)
				throw new NullReferenceException($"View prefab by path {entity.ViewPath} is null");

			EntityView view = FromPoolOrInstantiateSync(entity.ViewPath, prefab, position, rotation);
			if (view == null || entity.hasView)
				return null;

			SetupView(entity, view, entity.ViewPath, position, rotation);
			return view;
		}

		public EntityView CreateViewFromPrefab(GameEntity entity)
		{
			Vector3 position = GetSpawnPosition(entity);
			Quaternion rotation = GetSpawnRotation(entity);
			string viewKey = entity.ViewPrefab.GetInstanceID().ToString();

			EntityView view = FromPoolOrInstantiateSync(viewKey, entity.ViewPrefab, position, rotation);
			
			if (view == null || entity.hasView)
				return null;

			SetupView(entity, view, viewKey, position, rotation);
			return view;
		}

		public async UniTask<EntityView> CreateViewFromAssetReference(GameEntity entity)
		{
			Vector3 position = GetSpawnPosition(entity);
			Quaternion rotation = GetSpawnRotation(entity);
			string key = entity.ViewAssetReference.AssetGUID;

			entity.isLoadingView = true;
			try
			{
				EntityView view = await FromPoolOrInstantiateAsync(
					key,
					_assetsService.LoadAsync<GameObject>(entity.ViewAssetReference),
					position);

				if (view == null)
					return null;

				if (entity.GetComponents().Length <= 0 ||
				    entity.hasViewAssetReference == false ||
				    entity.hasView)
				{
					_viewPool.Put(view, key);
					return null;
				}

				SetupView(entity, view, view.gameObject.name, position, rotation);
				return view;
			}
			finally
			{
				entity.isLoadingView = false;
			}
		}

		public async UniTask<EntityView> CreateViewFromAddressableKey(GameEntity entity)
		{
			Vector3 position = GetSpawnPosition(entity);
			Quaternion rotation = GetSpawnRotation(entity);
			string key = entity.ViewAddressableKey;

			entity.isLoadingView = true;
			try
			{
				EntityView view = await FromPoolOrInstantiateAsync(
					key,
					_assetsService.LoadAsync<GameObject>(key),
					position);

				if (view == null)
					return null;

				if (entity.GetComponents().Length <= 0 || entity.hasView)
				{
					_viewPool.Put(view, key);
					return null;
				}

				SetupView(entity, view, view.gameObject.name, position, rotation);
				return view;
			}
			finally
			{
				entity.isLoadingView = false;
			}
		}

		private Vector3 GetSpawnPosition(GameEntity entity)
		{
			return entity.hasWorldPosition ? entity.WorldPosition : _farAway;
		}
		
		private Quaternion GetSpawnRotation(GameEntity entity)
		{
			return entity.hasWorldRotation ? entity.WorldRotation : Quaternion.identity;
		}

		private EntityView FromPoolOrInstantiateSync(string key, UnityEngine.Object prefab, Vector3 position, Quaternion rotation)
		{
			if (_viewPool.Has(key))
				return _viewPool.Get(key) as EntityView;

			EntityView instantiated = _instantiateService.InstantiatePrefabForComponent<EntityView>(prefab, position, rotation);
			instantiated.CaptureBaseScale();

			return instantiated;
		}

		private async UniTask<EntityView> FromPoolOrInstantiateAsync(string key, UniTask<GameObject> loadTask, Vector3 position)
		{
			if (_viewPool.Has(key))
				return _viewPool.Get(key) as EntityView;

			GameObject prefab = await loadTask;
			if (prefab == null)
				throw new NullReferenceException($"View prefab by key {key} is null");

			EntityView instantiated = _instantiateService.InstantiatePrefabForComponent<EntityView>(prefab, position, Quaternion.identity);
			instantiated.CaptureBaseScale();

			return instantiated;
		}

		private void SetupView(GameEntity entity, EntityView view, string parentKey, Vector3 position, Quaternion rotation)
		{
			view.transform.SetParent(GetParent(parentKey));
			view.AttachEntity(entity);
			view.transform.position = position;
			view.transform.rotation = rotation;

			if (entity.hasRigidbody)
			{
				RigidbodyInterpolation savedInterpolation = entity.Rigidbody.interpolation;
				entity.Rigidbody.interpolation = RigidbodyInterpolation.None;
				entity.Rigidbody.position = position;
				entity.Rigidbody.rotation = rotation;
				entity.Rigidbody.interpolation = savedInterpolation;
			}

			entity.ReplaceLossyScale(entity.hasLossyScale
				? Vector3.Scale(view.BaseScale, entity.LossyScale)
				: view.BaseScale);
		}

		private Transform GetParent(string name)
		{
			if (_generalParent == null)
			{
				_generalParent = new GameObject("Entity Views").transform;
				_generalParent.SetParent(ProjectContext.Instance.transform);
			}

			if (_concreteParents.TryGetValue(name, out Transform parent) && parent != null)
			{
				return parent;
			}

			Transform newParent = new GameObject(name.Replace("(Clone)", string.Empty) + " Parent").transform;
			newParent.SetParent(_generalParent);
			_concreteParents.Add(name, newParent);

			return newParent;
		}
	}
}
