using Code.Infrastructure.StateManagement.States;
using Framework.StateManagement;
using Framework.StateManagement.Factories;

namespace Code.Infrastructure.StateManagement
{
	public class GameStateMachine : StateMachine, IGameStateMachine
	{
		protected override string Name => nameof(GameStateMachine);
		protected override bool DebugLogsEnabled => true;

		public GameStateMachine(IStateFactory stateFactory) : base(stateFactory)
		{
		}

		public void RebootGame()
		{
			Enter<RebootAppState>();
		}
	}
}