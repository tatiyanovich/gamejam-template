using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Lifetime.Systems
{
	public class DestructOnDeathSystem : IExecuteSystem
	{
		private readonly GameContext _game;
		private readonly IGroup<GameEntity> _deathEvents;
		private readonly IGroup<GameEntity> _destructables;

		public DestructOnDeathSystem(GameContext game)
		{
			_game = game;
			_deathEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DeathEvent));

			_destructables = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DestructOnDeath));
		}

		public void Execute()
		{
			foreach (GameEntity eventEntity in _deathEvents)
			{
				GameEntity deadEntity = _game.GetEntityWithId(eventEntity.deathEvent.DeadEntityId);

				if (_destructables.ContainsEntity(deadEntity))
				{
					deadEntity.isDestructed = true;
				}
			}
		}
	}
}
