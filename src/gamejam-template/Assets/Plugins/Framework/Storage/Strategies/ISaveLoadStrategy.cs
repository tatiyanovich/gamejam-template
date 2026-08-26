using System;
using System.Collections.Generic;

namespace Framework.Storage.Strategies
{
	public interface ISaveLoadStrategy
	{
		bool HasSavedProgress(Dictionary<Type, ISaveFile> saveData);
		void Save(Dictionary<Type, ISaveFile> saveData);
		Dictionary<Type, ISaveFile> Load(List<Type> types);
		void EraseProgress(Dictionary<Type, ISaveFile> saveFilesByType);
	}
}
