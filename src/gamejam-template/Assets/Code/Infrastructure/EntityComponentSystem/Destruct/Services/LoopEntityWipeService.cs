using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Infrastructure.EntityComponentSystem.Destruct.Services
{
	public class LoopEntityWipeService : ILoopEntityWipeService
	{
		private readonly GameContext _game;
		private readonly ISystemFactory _systemFactory;

		public LoopEntityWipeService(GameContext game, ISystemFactory systemFactory)
		{
			_game = game;
			_systemFactory = systemFactory;
		}

		public void WipeNodeScopedEntities()
		{
			_game.MarkNodeScopedEntitiesDestructed();
			ProcessDestructed();
		}

		public void WipeNodeScopedEntities(LoopNodeId nodeId)
		{
			_game.MarkNodeScopedEntitiesDestructed(nodeId);
			ProcessDestructed();
		}

		private void ProcessDestructed()
		{
			ProcessDestructedFeature processDestructed = _systemFactory.Create<ProcessDestructedFeature>();

			processDestructed.Initialize();
			processDestructed.Execute();
			processDestructed.Cleanup();
			processDestructed.TearDown();
		}
	}
}
