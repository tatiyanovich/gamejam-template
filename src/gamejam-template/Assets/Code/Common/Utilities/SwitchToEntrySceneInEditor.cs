#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Code.Common.Utilities
{
	/// <summary>
	/// Should have such execution order, so it starts before every other script.
	/// </summary>
	[DefaultExecutionOrder(-32000)]
	public class SwitchToEntrySceneInEditor : MonoBehaviour
	{
#if UNITY_EDITOR
		private const int EntrySceneIndex = 0;

		private void Awake()
		{
			ProjectContext existing = FindFirstObjectByType<ProjectContext>();

			if (existing != null)
			{
				return;
			}
			
			string sceneAddress = SceneAddressablesUtilities.GetAddressByScenePath(gameObject.scene.path);
			EditorPrefs.SetString(Constants.EditorPrefsKeys.InitialScene, sceneAddress);

			foreach (GameObject root in gameObject.scene.GetRootGameObjects())
				root.SetActive(false);

			SceneManager.LoadScene(EntrySceneIndex);
		}
#endif
	}
}