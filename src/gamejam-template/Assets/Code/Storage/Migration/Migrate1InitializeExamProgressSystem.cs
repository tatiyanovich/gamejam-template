using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;
using UnityEngine;

namespace Code.Storage.Migration
{
	public class Migrate1InitializeExamProgressSystem : IExecuteSystem
	{
		private const int TargetVersion = 2;

		private readonly ISaveLoadService _saveLoadService;

		public Migrate1InitializeExamProgressSystem(ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();

			if (saveFile.SchemeVersion >= TargetVersion)
				return;

			saveFile.PlayerName ??= string.Empty;
			saveFile.IntroSeen = false;
			saveFile.BestAnswers = 0;
			saveFile.BestTimeSeconds = 0f;

			saveFile.SchemeVersion = TargetVersion;

			Debug.Log($"SaveFile migrated to SchemeVersion {TargetVersion}");
		}
	}
}
