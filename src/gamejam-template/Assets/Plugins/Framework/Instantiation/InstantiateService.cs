using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Framework.Instantiation
{
	public class InstantiateService : IInstantiateService, IInstantiatorDependant
	{
		private IInstantiator _instantiator;

		public InstantiateService(IInstantiator instantiator)
		{
			SetInstantiator(instantiator);
		}

		public void SetInstantiator(IInstantiator newInstantiator)
		{
			_instantiator = newInstantiator;
		}

		public T Instantiate<T>()
		{
			return _instantiator.Instantiate<T>();
		}

		public T Instantiate<T>(IEnumerable<object> args)
		{
			return _instantiator.Instantiate<T>(args);
		}

		public T Instantiate<T>(params object[] args)
		{
			return _instantiator.Instantiate<T>(args);
		}

		public T InstantiatePrefabForComponent<T>(Object prefab, Vector3 at, Quaternion rotation, Transform parent = null)
			where T : Component
		{
			T instantiated = _instantiator.InstantiatePrefabForComponent<T>(prefab, parent);
			instantiated.name = instantiated.name.Replace("(Clone)", string.Empty);
			Transform instantiatedTransform = instantiated.transform;
			instantiatedTransform.position = at;
			instantiatedTransform.rotation = rotation;

			return instantiated;
		}

		public T InstantiatePrefabForComponent<T>(Object prefab) where T : Component
		{
			return InstantiatePrefabForComponent<T>(prefab, Vector3.zero, Quaternion.identity);
		}

		public T Instantiate<T>(GameObject prefab, Vector3 at, Quaternion rotation, Transform parent = null)
			where T : Component
		{
			T instantiated = _instantiator.InstantiatePrefabForComponent<T>(prefab, parent);
			instantiated.name = instantiated.name.Replace("(Clone)", string.Empty);
			Transform instantiatedTransform = instantiated.transform;
			instantiatedTransform.position = at;
			instantiatedTransform.rotation = rotation;

			return instantiated;
		}

		public GameObject Instantiate(GameObject prefab, Vector3 at, Quaternion rotation, Transform parent = null)
		{
			GameObject instantiated = _instantiator.InstantiatePrefab(prefab, parent);
			instantiated.name = instantiated.name.Replace("(Clone)", string.Empty);
			Transform instantiatedTransform = instantiated.transform;
			instantiatedTransform.position = at;
			instantiatedTransform.rotation = rotation;

			return instantiated;
		}

		public T Instantiate<T>(GameObject prefab) where T : Component
		{
			T instantiated = _instantiator.InstantiatePrefabForComponent<T>(prefab);
			instantiated.name = instantiated.name.Replace("(Clone)", string.Empty);
			instantiated.transform.rotation = Quaternion.identity;

			return instantiated;
		}

		public T InstantiateComponent<T>(GameObject gameObject) where T : Component
		{
			return _instantiator.InstantiateComponent<T>(gameObject);
		}
	}
}
