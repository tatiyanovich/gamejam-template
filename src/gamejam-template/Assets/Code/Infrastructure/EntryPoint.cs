using Code.Infrastructure.StateManagement;
using Code.Infrastructure.StateManagement.States;
using Zenject;

namespace Code.Infrastructure
{
	public class EntryPoint : IInitializable
	{
		private readonly IGameStateMachine _gameStateMachine;

		public EntryPoint(IGameStateMachine gameStateMachine)
		{
			_gameStateMachine = gameStateMachine;
		}
		
		public void Initialize()
		{
			_gameStateMachine.Enter<BootstrapState>();
		}
	}
}