using System;
using System.Collections.Generic;
using System.Linq;
using Framework.Storage.Strategies;
using UnityEngine;
using Zenject;

namespace Framework.Storage
{
	public class SaveLoadService : ISaveLoadService, IInitializable
	{
		private readonly ISaveLoadStrategy _strategy;
		private readonly List<Type> _saveDataTypes;

		public Dictionary<Type, ISaveFile> SaveFilesByType { get; private set; }

		public event Action OnSaveRequested;
		public event Action OnSaveFinished;
		public event Action OnLoadFinished;

		public SaveLoadService(ISaveLoadStrategy strategy, List<Type> saveDataTypes)
		{
			_saveDataTypes = saveDataTypes;
			_strategy = strategy;

			InitializeSaveDataTypes(_saveDataTypes);
		}

		public void Initialize()
		{
		}

		public bool HasSavedProgress()
		{
			return _strategy.HasSavedProgress(SaveFilesByType);
		}

		public void SaveProgress()
		{
			OnSaveRequested?.Invoke();

			_strategy.Save(SaveFilesByType);

			OnSaveFinished?.Invoke();

			Debug.Log($"Progress saved ({DateTime.Now:HH:mm:ss yyyy-MM-dd})");
		}

		public void LoadProgress()
		{
			List<Type> types = SaveFilesByType.Keys.ToList();
			Dictionary<Type, ISaveFile> updatedData = _strategy.Load(types);

			foreach (KeyValuePair<Type, ISaveFile> pair in updatedData)
			{
				SaveFilesByType[pair.Key] = pair.Value;
			}

			OnLoadFinished?.Invoke();
			Debug.Log("Progress loaded");
		}

		public void EraseProgress()
		{
			_strategy.EraseProgress(SaveFilesByType);
			InitializeSaveDataTypes(_saveDataTypes);
			Debug.Log("Progress erased");
		}

		public T Get<T>() where T : class, ISaveFile
		{
			if (SaveFilesByType.ContainsKey(typeof(T)) == false)
				throw new ArgumentException($"Save data of type {typeof(T)} not found.");

			return (T)SaveFilesByType[typeof(T)];
		}

		private void InitializeSaveDataTypes(List<Type> saveDataTypes)
		{
			SaveFilesByType = new Dictionary<Type, ISaveFile>();

			foreach (Type type in saveDataTypes)
			{
				if (Activator.CreateInstance(type) is ISaveFile data)
					SaveFilesByType.Add(type, data);
			}
		}
	}
}
