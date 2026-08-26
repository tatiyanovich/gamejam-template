using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Infrastructure.ConfigsManagement;
using Code.Infrastructure.UiManagement.Services;
using Code.UI;
using Code.UI.Error;
using Code.UI.Fade;
using Code.UI.Gameplay;
using Code.UI.Joystick;
using Code.UI.Launch;
using Code.UI.Loading;
using Code.UI.Result;
using Code.UI.Settings;
using Cysharp.Threading.Tasks;
using Framework.Essentials.CursorManagement;
using Framework.StateManagement;
using Framework.UI.UiManagement;
using Framework.UI.UiManagement.Services;
using UnityEngine;

namespace Code.Infrastructure.StateManagement.States
{
	public class BootstrapState : IState, IEnter
	{
		private readonly IUiDefinitionService _uiDefinitionService;
		private readonly IGameStateMachine _gameStateMachine;
		private readonly IEnumerable<IConfigsService> _configs;
		private readonly ICursorLockService _cursorLockService;
		private readonly IUiService _uiService;
		private readonly IFallbackBaseCameraService _fallbackBaseCameraService;

		public BootstrapState(
			IUiDefinitionService uiDefinitionService,
			IGameStateMachine gameStateMachine,
			IEnumerable<IConfigsService> configs,
			ICursorLockService cursorLockService,
			IUiService uiService,
			IFallbackBaseCameraService fallbackBaseCameraService)
		{
			_uiService = uiService;
			_uiDefinitionService = uiDefinitionService;
			_gameStateMachine = gameStateMachine;
			_configs = configs;
			_cursorLockService = cursorLockService;
			_fallbackBaseCameraService = fallbackBaseCameraService;
		}

		public void Enter()
		{
			Initialize().Forget();
		}

		private async UniTaskVoid Initialize()
		{
			InitializeUI();
			await ShowLoadingWindow();

			_configs.ForEach(c => c.LoadConfigs());
			Debug.developerConsoleVisible = false;
			Time.maximumDeltaTime = 0.1f;

			LoadingWindow loadingWindow = _uiService.GetWindow<LoadingWindow>();
			loadingWindow.SetProgress(0.33f);

			_gameStateMachine.Enter<LoadProgressState>();
		}

		private void InitializeUI()
		{
			AddWindowDefinitions();
			_cursorLockService.SetLockPreference(LockPreferenceType.RequestMeansLock);
		}

		private async UniTask ShowLoadingWindow()
		{
			_fallbackBaseCameraService.EnableCamera();

			await UniTask.WhenAll(
				_uiService.OpenWindow<LoadingWindow>(),
				_uiService.OpenWindow<FadeWindow>());
		}

		// Every window needs a definition here before IUiService can open it: the address points at
		// a WindowConfig asset labelled 'window_configs', which in turn references the window prefab.
		private void AddWindowDefinitions()
		{
			_uiDefinitionService
				.AddDefinition(new WindowDefinition(typeof(LoadingWindow), Addresses.UI.LoadingWindow))
				.AddDefinition(new WindowDefinition(typeof(FadeWindow), Addresses.UI.FadeWindow))
				.AddDefinition(new WindowDefinition(typeof(LaunchWindow), Addresses.UI.LaunchWindow))
				.AddDefinition(new WindowDefinition(typeof(GameplayWindow), Addresses.UI.GameplayWindow))
				.AddDefinition(new WindowDefinition(typeof(WorldOverlayWindow), Addresses.UI.WorldOverlayWindow))
				.AddDefinition(new WindowDefinition(typeof(JoystickWindow), Addresses.UI.JoystickWindow))
				.AddDefinition(new WindowDefinition(typeof(ResultWindow), Addresses.UI.ResultWindow))
				.AddDefinition(new WindowDefinition(typeof(SettingsWindow), Addresses.UI.SettingsWindow))
				.AddDefinition(new WindowDefinition(typeof(ErrorWindow), Addresses.UI.ErrorWindow));
		}
	}
}
