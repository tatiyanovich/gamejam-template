using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Code.Editor
{
	public class ToolbarSaves
	{
		private const string SavesPath = "Assets/Saves";
		private const string ToolbarElementName = "Toolbar/Saves";

		[MainToolbarElement(ToolbarElementName, defaultDockPosition = MainToolbarDockPosition.Right)]
		private static MainToolbarButton CreateToolbarSaves()
		{
			Texture2D icon = EditorGUIUtility.FindTexture("d_TreeEditor.Trash");

			return new MainToolbarButton(
				new MainToolbarContent("", icon, "Delete all save data"),
				HandleDeleteClicked);
		}

		private static void HandleDeleteClicked()
		{
			if (AssetDatabase.IsValidFolder(SavesPath) == false)
				return;

			bool confirmed = EditorUtility.DisplayDialog(
				"Delete save data",
				"Are you sure you want to delete all save data?\n\nYou cannot undo the delete assets action",
				"Delete",
				"Cancel");

			if (confirmed == false)
				return;

			AssetDatabase.DeleteAsset(SavesPath);
			AssetDatabase.Refresh();

			PlayerPrefs.DeleteAll();
		}
	}
}
