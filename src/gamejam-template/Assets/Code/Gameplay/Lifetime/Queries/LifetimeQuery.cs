using System;
using Code.Infrastructure.EntityComponentSystem;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Lifetime.Queries
{
	public class LifetimeQuery : ILifetimeQuery, IReactiveQuery
	{
		private readonly GameContext _game;

		private readonly IGroup<GameEntity> _damageEvents;
		private readonly IGroup<GameEntity> _deathEvents;
		private readonly IGroup<GameEntity> _damageables;
		private readonly IGroup<GameEntity> _alive;

		public event Action<int, float> OnEntityDamaged;
		public event Action<int, float> OnEntityHealed;
		public event Action<int> OnEntityDied;

		public LifetimeQuery(GameContext game)
		{
			_game = game;
			_damageEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DamageEvent));

			_deathEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DeathEvent));

			_damageables = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Id,
					GameMatcher.CurrentHP,
					GameMatcher.MaxHP));

			_alive = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Id,
					GameMatcher.Alive));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity damageEvent in _damageEvents)
			{
				OnEntityDamaged?.Invoke(damageEvent.damageEvent.TargetEntityId, damageEvent.damageEvent.Amount);
			}

			foreach (GameEntity deathEvent in _deathEvents)
			{
				OnEntityDied?.Invoke(deathEvent.deathEvent.DeadEntityId);
			}
		}

		public float GetCurrentHp(int entityId)
		{
			GameEntity entity = _game.GetEntityWithId(entityId);

			if (_damageables.ContainsEntity(entity) == false)
				return 0f;

			return entity.CurrentHP;
		}

		public float GetMaxHp(int entityId)
		{
			GameEntity entity = _game.GetEntityWithId(entityId);

			if (_damageables.ContainsEntity(entity) == false)
				return 0f;

			return entity.MaxHP;
		}

		public bool IsFullHealth(int entityId)
		{
			GameEntity entity = _game.GetEntityWithId(entityId);

			if (_damageables.ContainsEntity(entity) == false)
				return false;

			return entity.CurrentHP >= entity.MaxHP;
		}

		public bool IsAlive(int entityId)
		{
			GameEntity entity = _game.GetEntityWithId(entityId);
			return _alive.ContainsEntity(entity);
		}
	}
}
