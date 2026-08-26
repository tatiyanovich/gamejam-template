using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Storage.Serialization;
using UnityEngine;

namespace Framework.Storage.Strategies
{
	public class PlayerPrefsSaveLoadStrategy : ISaveLoadStrategy
	{
		public bool HasSavedProgress(Dictionary<Type, ISaveFile> saveData)
		{
			return saveData.Keys.All(type => string.IsNullOrEmpty(PlayerPrefs.GetString(type.Name)) == false);
		}

		public void Save(Dictionary<Type, ISaveFile> saveData)
		{
			foreach (KeyValuePair<Type, ISaveFile> pair in saveData)
			{
				PlayerPrefs.SetString(pair.Key.Name, pair.Value.ToJson());
			}

			PlayerPrefs.Save();
		}

		public Dictionary<Type, ISaveFile> Load(List<Type> types)
		{
			Dictionary<Type, ISaveFile> updatedData = new();

			foreach (Type type in types)
			{
				if (PlayerPrefs.GetString(type.Name).FromJson(type) is ISaveFile data)
					updatedData[type] = data;
			}

			return updatedData;
		}

		public void EraseProgress(Dictionary<Type, ISaveFile> saveFilesByType)
		{
			foreach (Type type in saveFilesByType.Keys.ToList())
			{
				PlayerPrefs.DeleteKey(type.Name);
			}

			PlayerPrefs.Save();
		}
	}
}
