using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Extensions;

namespace Code.Infrastructure.EntityComponentSystem.Factories
{
	public class EntityFactory : IEntityFactory
	{
		private readonly GameContext _game;
		private readonly InputContext _input;
		private readonly ILoopNodeContext _loopNodeContext;

		public EntityFactory(GameContext game, InputContext input, ILoopNodeContext loopNodeContext)
		{
			_game = game;
			_input = input;
			_loopNodeContext = loopNodeContext;
		}

		public GameEntity Game() => StampNode(_game.CreateEntity());
		public GameEntity Event() => _game.CreateEntity().With(x => x.isEvent = true);
		public GameEntity Request() => _game.CreateEntity().With(x => x.isRequest = true);
		public InputEntity Input() => _input.CreateEntity().With(x => x.isInput = true);

		// Tags node-scoped entities with the branch currently ticking, so closing that branch
		// wipes only its own entities. Entities created outside a branch tick stay untagged.
		private GameEntity StampNode(GameEntity entity)
		{
			if (_loopNodeContext.Current != LoopNodeId.Unknown)
				entity.AddLoopNodeScope(_loopNodeContext.Current);

			return entity;
		}
	}
}
