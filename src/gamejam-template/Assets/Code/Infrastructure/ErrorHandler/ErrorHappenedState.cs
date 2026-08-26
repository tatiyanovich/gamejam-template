using System;
using Code.UI.Error;
using Cysharp.Threading.Tasks;
using Framework.StateManagement;
using Framework.UI.UiManagement.Services;
using UnityEngine;

namespace Code.Infrastructure.ErrorHandler
{
	public class ErrorHappenedState : IState, IEnter
	{
		private readonly IUiService _uiService;
		private readonly ILogGuardService _logGuardService;

		public ErrorHappenedState(
			IUiService uiService,
			ILogGuardService logGuardService)
		{
			_uiService = uiService;
			_logGuardService = logGuardService;
		}

		public void Enter()
		{
			OpenErrorWindow().Forget();
		}

		private async UniTaskVoid OpenErrorWindow()
		{
			// The ErrorWindow config/prefab may not exist in a stripped template. Degrade gracefully
			// instead of throwing a secondary exception from inside the error handler.
			try
			{
				await _uiService.OpenWindow<ErrorWindow>(
					beforeOpen: (window) =>
					{
						string logs = _logGuardService.GetRecentAsString(100);
						window.Setup(logs);
					});
			}
			catch (Exception exception)
			{
				Debug.LogWarning($"ErrorWindow could not be opened: {exception.Message}");
			}
		}
	}
}