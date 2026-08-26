using System.Collections.Generic;
using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class SteerRigidbody2DByHeadingSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;
		private readonly IGroup<GameEntity> _movers;
		private readonly List<GameEntity> _buffer = new(16);

		public SteerRigidbody2DByHeadingSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Rigidbody2D,
					GameMatcher.Velocity,
					GameMatcher.LookDirection,
					GameMatcher.Heading,
					GameMatcher.TurnSpeed,
					GameMatcher.MaxTurnRate,
					GameMatcher.TurnAcceleration,
					GameMatcher.TurnSpeedReference,
					GameMatcher.MinTurnFactor,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			float deltaTime = _timeService.FixedDeltaTime;

			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				float currentAngle = mover.Heading;
				Rigidbody2D body = mover.Rigidbody2D;

				float turnSpeedReference = Mathf.Max(0.01f, mover.TurnSpeedReference);
				float turnFactor = Mathf.Clamp(mover.Velocity.magnitude / turnSpeedReference, mover.MinTurnFactor, 1f);
				float desiredSpeed = ForwardTurnSpeed(mover, currentAngle) * turnFactor;

				float newSpeed = Mathf.MoveTowards(mover.TurnSpeed, desiredSpeed, mover.TurnAcceleration * deltaTime);
				float newAngle = currentAngle + newSpeed * deltaTime;

				mover.ReplaceTurnSpeed(newSpeed);
				mover.ReplaceHeading(newAngle);

				body.MoveRotation(newAngle);
			}
		}

		private float ForwardTurnSpeed(GameEntity mover, float currentAngle)
		{
			Vector3 lookDirection = mover.LookDirection;
			lookDirection.z = 0f;

			if (lookDirection.sqrMagnitude <= 0.0001f)
				return 0f;

			float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
			float error = Mathf.DeltaAngle(currentAngle, targetAngle);
			float stoppableSpeed = Mathf.Sqrt(2f * mover.TurnAcceleration * Mathf.Abs(error));

			return Mathf.Sign(error) * Mathf.Min(mover.MaxTurnRate, stoppableSpeed);
		}
	}
}
