using Code.Gameplay.CoreLoop;
using Code.Storage.SaveFiles;
using Entitas;
using Framework.Storage;
using UnityEngine;

namespace Code.Storage.Migration
{
	public class Migrate0InitializeSessionLoopStateSystem : IExecuteSystem
	{
		private const int TargetVersion = 1;

		private readonly ISaveLoadService _saveLoadService;

		public Migrate0InitializeSessionLoopStateSystem(ISaveLoadService saveLoadService)
		{
			_saveLoadService = saveLoadService;
		}

		public void Execute()
		{
			GeneralSaveFile saveFile = _saveLoadService.Get<GeneralSaveFile>();

			if (saveFile.SchemeVersion >= TargetVersion)
				return;

			saveFile.SessionLoop ??= SessionLoopSnapshot.CreateForLegacyProgress();

			saveFile.SchemeVersion = TargetVersion;

			Debug.Log($"SaveFile migrated to SchemeVersion {TargetVersion}");
		}
	}
}
