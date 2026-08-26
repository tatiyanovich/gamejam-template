#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Framework.Storage.Serialization;
using UnityEditor;
using UnityEngine;

namespace Framework.Storage.Strategies
{
	public class EditorFileSaveLoadStrategy : ISaveLoadStrategy
	{
		private const string EditorSaveFolder = "Saves";
		private readonly string _editorSavePath = Path.Combine(Application.dataPath, EditorSaveFolder);

		public bool HasSavedProgress(Dictionary<Type, ISaveFile> saveData)
		{
			EnsureSaveFolderExists();
			string[] files = Directory.GetFiles(_editorSavePath);
			string[] jsonFiles = Array.FindAll(files, f => f.EndsWith(".json"));
			return jsonFiles.Length > 0;
		}

		public void Save(Dictionary<Type, ISaveFile> saveData)
		{
			EnsureSaveFolderExists();
			foreach (KeyValuePair<Type, ISaveFile> pair in saveData)
			{
				File.WriteAllText(GetEditorSavePath(pair.Key.Name), pair.Value.ToJson());
			}
		}

		public Dictionary<Type, ISaveFile> Load(List<Type> types)
		{
			Dictionary<Type, ISaveFile> updatedData = new();
			EnsureSaveFolderExists();

			foreach (Type type in types)
			{
				string json = null;
				if (File.Exists(GetEditorSavePath(type.Name)))
					json = File.ReadAllText(GetEditorSavePath(type.Name));

				try
				{
					if (string.IsNullOrEmpty(json) == false && json.FromJson(type) is ISaveFile data)
						updatedData[type] = data;
				}
				catch (Exception e)
				{
					Debug.LogError($"Failed to load save data for {type.Name}: {e.Message}");
				}
			}

			return updatedData;
		}

		public void EraseProgress(Dictionary<Type, ISaveFile> saveFilesByType)
		{
			EnsureSaveFolderExists();
			Directory.Delete(_editorSavePath, true);
			AssetDatabase.Refresh();
		}

		private void EnsureSaveFolderExists()
		{
			if (Directory.Exists(_editorSavePath) == false)
			{
				Directory.CreateDirectory(_editorSavePath);
				AssetDatabase.Refresh();
			}
		}

		private string GetEditorSavePath(string fileName)
		{
			return Path.Combine(_editorSavePath, fileName + ".json");
		}
	}
}
#endif
