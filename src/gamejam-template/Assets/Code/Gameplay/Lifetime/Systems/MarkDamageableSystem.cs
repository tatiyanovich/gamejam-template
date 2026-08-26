using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Lifetime.Systems
{
	public sealed class MarkDamageableSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _aliveChangedEntities;

		private readonly List<GameEntity> _buffer = new(64);

		public MarkDamageableSystem(GameContext game)
		{
			_aliveChangedEntities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Id,
					GameMatcher.CurrentHP,
					GameMatcher.MaxHP,
					GameMatcher.WorldPosition,
					GameMatcher.AliveChanged));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _aliveChangedEntities.GetEntities(_buffer))
			{
				entity.isDamageable = entity.isAlive;
			}
		}
	}
}
