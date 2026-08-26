using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class ApplyImpulseSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _entities;
		private readonly List<GameEntity> _buffer = new(32);

		public ApplyImpulseSystem(GameContext game)
		{
			_entities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Impulse,
					GameMatcher.Rigidbody2D));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _entities.GetEntities(_buffer))
			{
				entity.Rigidbody2D.AddForce(entity.Impulse, ForceMode2D.Impulse);
				entity.RemoveImpulse();
			}
		}
	}
}
