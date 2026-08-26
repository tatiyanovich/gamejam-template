using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Storage.Migration;
using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement.States
{
	public class MigrateProgressState : IState, IEnter
	{
		private readonly IGameStateMachine _gameStateMachine;
		private readonly ISystemFactory _systemFactory;

		public MigrateProgressState(
			IGameStateMachine gameStateMachine,
			ISystemFactory systemFactory)
		{
			_gameStateMachine = gameStateMachine;
			_systemFactory = systemFactory;
		}

		public void Enter()
		{
			MigrationFeature migrationFeature = _systemFactory.Create<MigrationFeature>();

			migrationFeature.Initialize();
			migrationFeature.Execute();
			migrationFeature.Cleanup();
			migrationFeature.TearDown();

			_gameStateMachine.Enter<PrepareAssetsState>();
		}
	}
}
