using Code.Infrastructure.CoreLoop;
using Code.UI.Launch;
using Code.UI.Loading;
using Cysharp.Threading.Tasks;
using Framework.StateManagement;
using Framework.UI.UiManagement.Services;

namespace Code.Infrastructure.StateManagement.States
{
	// Per-node preparation after its scene is loaded but before its pipeline starts: open the
	// windows that node owns, warm up whatever it needs, then hand over to RunLoopSceneState.
	public class PrepareLoopSceneState : IState, IPayloadedEnter<LoopScenePayload>
	{
		private readonly IGameStateMachine _gameStateMachine;
		private readonly IUiService _uiService;

		public PrepareLoopSceneState(
			IGameStateMachine gameStateMachine,
			IUiService uiService)
		{
			_gameStateMachine = gameStateMachine;
			_uiService = uiService;
		}

		public void Enter(LoopScenePayload loopScenePayload)
		{
			Prepare(loopScenePayload).Forget();
		}

		private async UniTaskVoid Prepare(LoopScenePayload loopScenePayload)
		{
			if (loopScenePayload.LoopNodeId == LoopNodeId.StartLaunch)
				await _uiService.OpenWindow<LaunchWindow>();

			LoadingWindow loadingWindow = _uiService.GetWindow<LoadingWindow>();

			if (loadingWindow)
				loadingWindow.SetProgress(1f);

			_gameStateMachine.Enter<RunLoopSceneState, LoopScenePayload>(loopScenePayload);

			await _uiService.CloseWindow<LoadingWindow>();
		}
	}
}
