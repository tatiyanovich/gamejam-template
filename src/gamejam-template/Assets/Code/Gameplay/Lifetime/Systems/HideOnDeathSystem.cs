using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Lifetime.Systems
{
	public class HideOnDeathSystem : IExecuteSystem
	{
		private readonly GameContext _game;
		
		private readonly IGroup<GameEntity> _deathEvents;
		private readonly IGroup<GameEntity> _destructables;

		public HideOnDeathSystem(GameContext game)
		{
			_game = game;
			_deathEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DeathEvent));

			_destructables = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.HideOnDeath,
					GameMatcher.View));
		}

		public void Execute()
		{
			foreach (GameEntity eventEntity in _deathEvents)
			{
				GameEntity deadEntity = _game.GetEntityWithId(eventEntity.deathEvent.DeadEntityId);

				if (_destructables.ContainsEntity(deadEntity))
				{
					deadEntity.View.gameObject.SetActive(false);
				}
			}
		}
	}
}
