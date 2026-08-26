using System;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Framework.Essentials.SceneManagement
{
	public interface ISceneLoadService
	{
		UniTask<SceneInstance> LoadScene(string name, LoadSceneMode loadSceneMode, Action onLoaded = null);
		UniTask UnloadScene(SceneInstance sceneInstance, Action onUnloaded = null);
		event Action OnSceneLoadStarted;
	}
}
