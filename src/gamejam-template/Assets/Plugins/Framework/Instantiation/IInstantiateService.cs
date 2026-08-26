using System.Collections.Generic;
using UnityEngine;

namespace Framework.Instantiation
{
	public interface IInstantiateService
	{
		T Instantiate<T>();
		T Instantiate<T>(IEnumerable<object> args);
		T Instantiate<T>(params object[] args);
		T Instantiate<T>(GameObject prefab, Vector3 at, Quaternion rotation, Transform parent = null) where T : Component;
		T InstantiatePrefabForComponent<T>(Object prefab, Vector3 at, Quaternion rotation, Transform parent = null) where T : Component;
		T InstantiatePrefabForComponent<T>(Object prefab) where T : Component;

		GameObject Instantiate(GameObject prefab, Vector3 at, Quaternion rotation, Transform parent = null);
		T Instantiate<T>(GameObject prefab) where T : Component;
		T InstantiateComponent<T>(GameObject gameObject) where T : Component;
	}
}
