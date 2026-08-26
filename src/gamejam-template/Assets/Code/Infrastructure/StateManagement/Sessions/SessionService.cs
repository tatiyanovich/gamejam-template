using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.StateManagement.Branching;
using Code.Infrastructure.StateManagement.Sessions.SessionStates;
using Code.Infrastructure.StateManagement.States;
using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.StateManagement.Sessions
{
	public class SessionService : ISessionService
	{
		private readonly IBranchedStateMachine _branchedStateMachine;
		private readonly ISessionWindowsPresenter _windowsPresenter;
		private readonly ISessionRevealGate _revealGate;

		public SessionService(
			IBranchedStateMachine branchedStateMachine,
			ISessionWindowsPresenter windowsPresenter,
			ISessionRevealGate revealGate)
		{
			_branchedStateMachine = branchedStateMachine;
			_windowsPresenter = windowsPresenter;
			_revealGate = revealGate;
		}

		public void EnterNode(LoopScenePayload payload)
		{
			_branchedStateMachine.SetActiveNode(payload.LoopNodeId);

			if (IsRunning(payload.LoopNodeId))
			{
				ResumeSession(payload.LoopNodeId).Forget();
				return;
			}

			_branchedStateMachine.OpenBranch(payload.LoopNodeId)
				.Enter<PreloadSessionState, LoopScenePayload>(payload);
		}

		public void CloseSession(LoopNodeId nodeId)
		{
			_branchedStateMachine.CloseBranch(nodeId);
		}

		public void CloseAll()
		{
			_branchedStateMachine.CloseAll();
		}

		private bool IsRunning(LoopNodeId nodeId)
		{
			return _branchedStateMachine.TryGetBranch(nodeId, out IStateBranch branch)
			       && branch.Current is RunSessionState;
		}

		private async UniTaskVoid ResumeSession(LoopNodeId nodeId)
		{
			_revealGate.RegisterPending();

			await _windowsPresenter.Present(nodeId);

			_revealGate.SetNextRevealDelay(0f);
			_revealGate.NotifyReady();
		}
	}
}
