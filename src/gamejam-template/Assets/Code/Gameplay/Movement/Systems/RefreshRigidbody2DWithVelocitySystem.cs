using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class RefreshRigidbody2DWithVelocitySystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;
		private readonly List<GameEntity> _buffer = new(16);

		public RefreshRigidbody2DWithVelocitySystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RigidbodyVelocity2DMovement,
					GameMatcher.Rigidbody2D,
					GameMatcher.Velocity,
					GameMatcher.VelocityChanged));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				mover.Rigidbody2D.linearVelocity = (Vector2)mover.Velocity;
			}
		}
	}
}
