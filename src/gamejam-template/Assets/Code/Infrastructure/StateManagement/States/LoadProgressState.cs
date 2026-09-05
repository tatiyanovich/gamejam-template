using Code.Common.Extensions;
using Code.Gameplay.CoreLoop;
using Code.Infrastructure.AppMetadata.Snapshots;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Code.Infrastructure.Settings.Services;
using Code.Storage.SaveFiles;
using Framework.Essentials.TimeManagement;
using Framework.StateManagement;
using Framework.Storage;
using UnityEngine;

namespace Code.Infrastructure.StateManagement.States
{
	public class LoadProgressState : IState, IEnter
	{
		private const int CurrentSaveSchemeVersion = 2;

		private readonly IGameStateMachine _gameStateMachine;
		private readonly ISaveLoadService _saveLoadService;
		private readonly IIdentifierService _identifierService;
		private readonly ITimeService _timeService;
		private readonly ISettingsService _settingsService;

		public LoadProgressState(
			IGameStateMachine gameStateMachine,
			ISaveLoadService saveLoadService,
			IIdentifierService identifierService,
			ITimeService timeService,
			ISettingsService settingsService)
		{
			_gameStateMachine = gameStateMachine;
			_saveLoadService = saveLoadService;
			_identifierService = identifierService;
			_timeService = timeService;
			_settingsService = settingsService;
		}

		public void Enter()
		{
			InitializeProgress();
			_gameStateMachine.Enter<MigrateProgressState>();
		}

		private void InitializeProgress()
		{
			if (_saveLoadService.HasSavedProgress() == false)
			{
				CreateNewProgress();
			}

			LoadProgress();
		}

		private void CreateNewProgress()
		{
			AppMetadataSnapshot appMetadataSnapshot = new(
				firstLaunchUnixTime: _timeService.UtcNow.ToUnixTimeSeconds(),
				lastUsedVersion: Application.version,
				lastUsedId: 0
			);

			GeneralSaveFile generalSaveFile = _saveLoadService.Get<GeneralSaveFile>();
			generalSaveFile.SchemeVersion = CurrentSaveSchemeVersion;
			generalSaveFile.AppMetadata = appMetadataSnapshot;
			generalSaveFile.SessionLoop = SessionLoopSnapshot.CreateForNewProgress();
			generalSaveFile.PlayerName = string.Empty;
			generalSaveFile.IntroSeen = false;
			generalSaveFile.BestAnswers = 0;
			generalSaveFile.BestTimeSeconds = 0f;

			_saveLoadService.SaveProgress();

			Debug.Log("New progress created");
		}

		private void LoadProgress()
		{
			_saveLoadService.LoadProgress();

			GeneralSaveFile generalSaveFile = _saveLoadService.Get<GeneralSaveFile>();

			_identifierService.SetLastUsedId(generalSaveFile.AppMetadata.LastUsedId);
			_settingsService.LoadProgress();
		}
	}
}
