using System;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.Health;
using Framework.Storage;
using UnityEngine;
using Zenject;

namespace Code.Storage.Services
{
	public class RefreshStorageService : MonoBehaviour, IInitializable, IDisposable
	{
		private ISaveLoadService _saveLoadService;
		private ISystemFactory _systemFactory;
		private IApplicationHealthService _applicationHealthService;

		private RefreshSnapshotsFeature _refreshSnapshotsFeature;

		[Inject]
		private void Construct(
			ISaveLoadService saveLoadService,
			ISystemFactory systemFactory,
			IApplicationHealthService applicationHealthService)
		{
			_saveLoadService = saveLoadService;
			_systemFactory = systemFactory;
			_applicationHealthService = applicationHealthService;
		}

		public void Initialize()
		{
			_refreshSnapshotsFeature = _systemFactory.Create<RefreshSnapshotsFeature>();
			_refreshSnapshotsFeature.Initialize();

			_saveLoadService.OnSaveRequested += HandleSaveRequested;
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (isPaused == false)
				return;

			SaveProgressSafely();
		}

		private void OnApplicationQuit()
		{
			SaveProgressSafely();
		}

		public void Dispose()
		{
			_refreshSnapshotsFeature.TearDown();
			_refreshSnapshotsFeature = null;

			_saveLoadService.OnSaveRequested -= HandleSaveRequested;
		}

		private void SaveProgressSafely()
		{
			if (_applicationHealthService.HasCriticalErrors)
			{
				Debug.LogError("Skipping save due to critical errors during session.");
				return;
			}

			try
			{
				_saveLoadService.SaveProgress();
			}
			catch (Exception exception)
			{
				Debug.LogError($"Save failed: {exception.Message}");
			}
		}

		private void HandleSaveRequested()
		{
			_refreshSnapshotsFeature.Execute();
			_refreshSnapshotsFeature.Cleanup();
		}
	}
}
