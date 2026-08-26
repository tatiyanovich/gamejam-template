using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Editor
{
	[InitializeOnLoad]
	public class ToolbarScenes
	{
		private static string activeSceneName;
		private static readonly Dictionary<string, string> _scenePathsByName = new();
		
		static ToolbarScenes()
		{
			EditorBuildSettings.sceneListChanged -= RefreshSceneList;
			EditorBuildSettings.sceneListChanged += RefreshSceneList;

			EditorSceneManager.sceneOpened -= HandleSceneChanged;
			EditorSceneManager.sceneOpened += HandleSceneChanged;
		}

		private const string ToolbarElementName = "ToolbarScenes/SceneSwitcher";

		[MainToolbarElement(ToolbarElementName, defaultDockPosition = MainToolbarDockPosition.Middle)]
		private static IEnumerable<MainToolbarElement> CreateSceneSwitcher()
		{			
			RefreshSceneList();

			Texture2D unityIcon = EditorGUIUtility.FindTexture("UnityLogo");
			
			yield return new MainToolbarDropdown(
				new MainToolbarContent(activeSceneName, unityIcon, "Switch between build scenes"),
				HandleDropdownOpened);
		}

		private static void HandleDropdownOpened(Rect dropDownRect)
		{
			GenericMenu menu = new();

			foreach (KeyValuePair<string, string> scene in _scenePathsByName)
			{
				string scenePath = scene.Value;
				menu.AddItem(new GUIContent(scene.Key), false, () => OpenScene(scenePath));
			}

			menu.DropDown(dropDownRect);
		}

		private static void OpenScene(string scenePath)
		{
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
			{
				EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
			}
		}

		private static void HandleSceneChanged(Scene previousScene, OpenSceneMode mode)
		{
			activeSceneName = SceneManager.GetActiveScene().name;
			MainToolbar.Refresh(ToolbarElementName);
		}

		private static void RefreshSceneList()
		{
			_scenePathsByName.Clear();

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
			{
				if (string.IsNullOrEmpty(scene.path) || scene.path.StartsWith("Assets") == false)
					continue;

				string sceneName = Path.GetFileNameWithoutExtension(scene.path);
				_scenePathsByName[sceneName] = scene.path;
			}
			
			activeSceneName = SceneManager.GetActiveScene().name;
		}
	}
}