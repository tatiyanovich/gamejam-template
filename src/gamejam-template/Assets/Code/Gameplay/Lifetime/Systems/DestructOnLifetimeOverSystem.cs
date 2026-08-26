using Entitas;

namespace Code.Gameplay.Lifetime.Systems
{
	public class DestructOnLifetimeOverSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _entities;

		public DestructOnLifetimeOverSystem(GameContext game)
		{
			_entities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.LifetimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _entities)
			{
				if (entity.LifetimeLeft <= 0)
					entity.isDestructed = true;
			}
		}
	}
}
