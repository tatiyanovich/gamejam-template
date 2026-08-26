using System;
using Code.Infrastructure.CoreLoop;
using Code.UI;
using Code.UI.Gameplay;
using Code.UI.Joystick;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Services;

namespace Code.Infrastructure.StateManagement.Sessions
{
	// Owns which windows belong to which loop node. Add a branch per node you introduce —
	// closing the previous node's windows here is what keeps sessions from stacking HUDs.
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

		private UniTask PresentBattle()
		{
			return UniTask.WhenAll(
				_uiService.OpenWindow<WorldOverlayWindow>(),
				_uiService.OpenWindow<GameplayWindow>(),
				_uiService.OpenWindow<JoystickWindow>());
		}
	}
}
