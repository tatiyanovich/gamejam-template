using System;
using Cysharp.Threading.Tasks;
using Framework.StateManagement;
using Framework.UI.UiManagement.Services;

namespace Code.Infrastructure.StateManagement.States
{
	public class RebootAppState : IState, IEnter
	{
		private readonly IGameStateMachine _gameStateMachine;
		private readonly IUiService _uiService;
		private readonly IUiDefinitionService _uiDefinitionService;

		public RebootAppState(
			IGameStateMachine gameStateMachine,
			IUiService uiService,
			IUiDefinitionService uiDefinitionService)
		{
			_gameStateMachine = gameStateMachine;
			_uiService = uiService;
			_uiDefinitionService = uiDefinitionService;
		}

		public void Enter()
		{
			Reboot().Forget();
		}

		private async UniTaskVoid Reboot()
		{
			await _uiService.CloseAllWindows(withAnimation: false);
			_uiDefinitionService.ClearAllDefinitions();
			
			_gameStateMachine.Enter<CleanupAllContextsState, Action>(_gameStateMachine.Enter<BootstrapState>);
		}
	}
}