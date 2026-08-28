using System;
using Code.Infrastructure.CoreLoop;
using Code.UI;
using Code.UI.Gameplay;
using Code.UI.Joystick;
using Code.UI.Launch;
using Code.UI.Result;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Services;

namespace Code.Infrastructure.StateManagement.Sessions
{
	public class SessionWindowsPresenter : ISessionWindowsPresenter
	{
		private readonly IUiService _uiService;

		public SessionWindowsPresenter(IUiService uiService)
		{
			_uiService = uiService;
		}

		public UniTask Present(LoopNodeId nodeId)
		{
			switch (nodeId)
			{
				case LoopNodeId.Battle:
					return PresentBattle();
				default:
					throw new ArgumentOutOfRangeException(nameof(nodeId), nodeId, null);
			}
		}

		public UniTask Dismiss(LoopNodeId nodeId)
		{
			switch (nodeId)
			{
				case LoopNodeId.Battle:
					return DismissBattle();
				default:
					throw new ArgumentOutOfRangeException(nameof(nodeId), nodeId, null);
			}
		}

		private async UniTask PresentBattle()
		{
			await UniTask.WhenAll(
				_uiService.CloseWindow<LaunchWindow>(withAnimation: false),
				_uiService.CloseWindow<ResultWindow>(withAnimation: false));

			await UniTask.WhenAll(
				_uiService.OpenWindow<WorldOverlayWindow>(),
				_uiService.OpenWindow<GameplayWindow>(),
				_uiService.OpenWindow<JoystickWindow>());
		}

		private UniTask DismissBattle()
		{
			return UniTask.WhenAll(
				_uiService.CloseWindow<ResultWindow>(withAnimation: false),
				_uiService.CloseWindow<JoystickWindow>(withAnimation: false),
				_uiService.CloseWindow<GameplayWindow>(withAnimation: false),
				_uiService.CloseWindow<WorldOverlayWindow>(withAnimation: false));
		}
	}
}
