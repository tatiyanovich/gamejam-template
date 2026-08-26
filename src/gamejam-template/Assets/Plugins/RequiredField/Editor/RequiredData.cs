using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Plugins.RequiredField.Editor
{
	public class RequiredData : ScriptableObject
	{
		public bool ShowIcon = true;
		public bool ShowBorder = true;
		public IconTypeId IconTypeId = IconTypeId.Error;
		public Color BorderColor = Color.red;
		
		private static RequiredData data;

		[SerializeField, HideInInspector] private List<string> disabledKeys = new();

		private const string LocalPath = "Assets/Resources";

		public static RequiredData Data
		{
			get
			{
				if (data == null)
				{
					data = (RequiredData) AssetDatabase.LoadAssetAtPath($"{LocalPath}/RequiredData.asset",
						typeof(RequiredData));

					if (data == null)
					{
						if (AssetDatabase.IsValidFolder(LocalPath) == false)
						{
							AssetDatabase.CreateFolder("Assets", "Resources");
						}

						RequiredData dataFile = CreateInstance<RequiredData>();
						string path = $"{LocalPath}/RequiredData.asset";
						AssetDatabase.CreateAsset(dataFile, path);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();

						data = dataFile;
						return data;
					}
				}

				return data;
			}
		}

		public string GetIconType()
		{
			return IconTypeId switch
			{
				IconTypeId.Error => "d_console.erroricon",
				IconTypeId.Warning => "d_console.warnicon",
				IconTypeId.Info => "d_console.infoicon",
				_ => ""
			};
		}

		public bool TryGetKey(string key)
		{
			return disabledKeys.Contains(key);
		}

		public void AddKey(string key)
		{
			if (TryGetKey(key) == false)
			{
				disabledKeys.Add(key);
			}
		}

		public void RemoveKey(string key)
		{
			disabledKeys.Remove(key);
		}
	}
}