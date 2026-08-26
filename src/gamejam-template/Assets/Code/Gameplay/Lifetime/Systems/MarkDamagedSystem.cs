using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Lifetime.Systems
{
	public sealed class MarkDamagedSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _healthChangedEntities;

		private readonly List<GameEntity> _buffer = new(64);

		public MarkDamagedSystem(GameContext game)
		{
			_healthChangedEntities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.CurrentHP,
					GameMatcher.MaxHP,
					GameMatcher.CurrentHPChanged));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _healthChangedEntities.GetEntities(_buffer))
			{
				entity.isDamaged = entity.CurrentHP < entity.MaxHP;
			}
		}
	}
}
