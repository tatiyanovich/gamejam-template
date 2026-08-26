using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Framework.Essentials.SceneManagement
{
	public class SceneLoadService : ISceneLoadService
	{
		public event Action OnSceneLoadStarted;

		public async UniTask<SceneInstance> LoadScene(string name, LoadSceneMode loadSceneMode, Action onLoaded = null)
		{
			OnSceneLoadStarted?.Invoke();
			return await Load(name, loadSceneMode, onLoaded);
		}

		public async UniTask UnloadScene(SceneInstance sceneInstance, Action onUnloaded = null)
		{
			if (sceneInstance.Scene.IsValid() == false || sceneInstance.Scene.isLoaded == false)
			{
				onUnloaded?.Invoke();
				return;
			}

			// Unity refuses to unload the last remaining scene ("Unloading the last loaded scene … is not
			// supported"). Skip instead of throwing an unobserved exception into the global error state;
			// callers unload while at least one other scene is loaded.
			if (SceneManager.loadedSceneCount <= 1)
			{
				onUnloaded?.Invoke();
				return;
			}

			// autoReleaseHandle:false so the handle stays valid after completion — otherwise Addressables
			// auto-releases it the moment the unload finishes and reading .Status below throws
			// "Attempting to use an invalid operation handle".
			AsyncOperationHandle<SceneInstance> waitUnloadScene =
				Addressables.UnloadSceneAsync(sceneInstance, autoReleaseHandle: false);

			// Addressables returns an invalid handle when it no longer tracks the scene (e.g. a scene
			// loaded in Single mode whose load handle was already released). Fall back to the
			// SceneManager so the scene still unloads instead of throwing.
			if (waitUnloadScene.IsValid() == false)
			{
				await SceneManager.UnloadSceneAsync(sceneInstance.Scene).ToUniTask();
				onUnloaded?.Invoke();
				return;
			}

			await UniTask.WaitUntil(() => waitUnloadScene.IsDone);

			bool succeeded = waitUnloadScene.Status == AsyncOperationStatus.Succeeded;
			Addressables.Release(waitUnloadScene);

			if (succeeded == false)
				throw new Exception($"Scene {sceneInstance.Scene.name} unloading failed");

			onUnloaded?.Invoke();
		}

		private async UniTask<SceneInstance> Load(string nextScene, LoadSceneMode loadSceneMode, Action onLoaded = null)
		{
			SceneInstance sceneInstance;

			AsyncOperationHandle<SceneInstance> waitNextScene = Addressables.LoadSceneAsync(nextScene, loadSceneMode);

			await UniTask.WaitUntil(() => waitNextScene.IsDone);

			if (waitNextScene.Status == AsyncOperationStatus.Succeeded)
			{
				sceneInstance = await waitNextScene;
				await sceneInstance.ActivateAsync().ToUniTask();
			}
			else
			{
				throw new Exception($"Scene {nextScene} loading failed");
			}

			onLoaded?.Invoke();

			return sceneInstance;
		}
	}
}
