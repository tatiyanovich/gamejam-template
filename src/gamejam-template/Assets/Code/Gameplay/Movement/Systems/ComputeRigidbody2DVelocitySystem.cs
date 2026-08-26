using System.Collections.Generic;
using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class ComputeRigidbody2DVelocitySystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;
		private readonly IGroup<GameEntity> _movers;
		private readonly List<GameEntity> _buffer = new(16);

		public ComputeRigidbody2DVelocitySystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RigidbodyVelocity2DMovement,
					GameMatcher.MovementDirection,
					GameMatcher.MovementSpeed,
					GameMatcher.MinMovementSpeed,
					GameMatcher.MovementSpeedMultiplier,
					GameMatcher.CanMove,
					GameMatcher.Velocity));
		}

		public void Execute()
		{
			float deltaTime = _timeService.FixedDeltaTime;

			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				float speed = Mathf.Max(
					mover.MovementSpeed * mover.MovementSpeedMultiplier,
					mover.MinMovementSpeed);

				Vector3 targetVelocity = mover.MovementDirection * speed;

				if (mover.hasMovementAcceleration)
				{
					targetVelocity = Vector3.MoveTowards(
						mover.Velocity,
						targetVelocity,
						mover.MovementAcceleration * deltaTime);
				}

				mover.ReplaceVelocity(targetVelocity);
			}
		}
	}
}
