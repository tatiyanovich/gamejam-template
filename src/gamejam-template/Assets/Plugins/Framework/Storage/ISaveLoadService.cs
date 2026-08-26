using System;
using System.Collections.Generic;

namespace Framework.Storage
{
	public interface ISaveLoadService
	{
		Dictionary<Type, ISaveFile> SaveFilesByType { get; }
		event Action OnSaveRequested;
		event Action OnSaveFinished;
		void SaveProgress();
		void LoadProgress();
		T Get<T>() where T : class, ISaveFile;
		bool HasSavedProgress();
		event Action OnLoadFinished;
		void EraseProgress();
	}
}
