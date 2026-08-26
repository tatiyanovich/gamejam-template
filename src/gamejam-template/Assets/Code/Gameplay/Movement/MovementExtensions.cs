using Code.Infrastructure.EntityComponentSystem.Extensions;
using UnityEngine;

namespace Code.Gameplay.Movement
{
	public static class MovementExtensions
	{
		public static GameEntity SetupRigidbodyInterpolatedMovement(
			this GameEntity entity,
			float speed,
			float slowDownModifier,
			float speedUpModifier,
			Vector3 worldPosition)
		{
			entity.UnSetupAllMovements();
			
			return entity
					.With(x => x.isRigidbodyInterpolatedMovement = true)
					.AddMovementDirection(Vector3.zero)
					.ReplaceVelocity(Vector3.zero)
					.AddMovementSlowDownModifier(slowDownModifier)
					.AddMovementSpeedUpModifier(speedUpModifier)
					.AddMovementSpeed(speed)
					.AddWorldPosition(worldPosition)
				;
		}

		public static GameEntity SetupRigidbodyVelocityMovement(
			this GameEntity entity,
			float speed,
			Vector3 worldPosition,
			Vector3 movementDirection)
		{
			entity.UnSetupAllMovements();

			entity
				.With(x => x.isRigidbodyVelocityMovement = true)
				.ReplaceWorldPosition(worldPosition)
				.With(x => x.ReplaceMovementSpeed(speed), when: speed > 0)
				.ReplaceMovementDirection(movementDirection)
				.ReplaceVelocity(Vector3.zero)
				;

			return entity;
		}

		public static GameEntity SetupRigidbodyPositionMovement(this GameEntity entity, Vector3 worldPosition)
		{
			entity.UnSetupAllMovements();

			entity
				.With(x => x.isRigidbodyPositionMovement = true)
				.ReplaceWorldPosition(worldPosition)
				;

			return entity;
		}

		public static GameEntity SetupRigidbodyVelocity2DMovement(this GameEntity entity, Vector3 worldPosition)
		{
			entity.UnSetupAllMovements();

			entity
				.With(x => x.isRigidbodyVelocity2DMovement = true)
				.ReplaceWorldPosition(worldPosition)
				.ReplaceMovementDirection(Vector3.zero)
				.ReplaceVelocity(Vector3.zero)
				;

			return entity;
		}

		public static GameEntity SetupTransformMovement(this GameEntity entity, Vector3 worldPosition)
		{
			entity.UnSetupAllMovements();

			entity
				.With(x => x.isTransformMovement = true)
				.ReplaceWorldPosition(worldPosition)
				.ReplaceMovementDirection(Vector2.zero)
				;

			return entity;
		}

		public static GameEntity SetupLateTransformMovement(this GameEntity entity, Vector3 worldPosition)
		{
			entity.UnSetupAllMovements();

			entity
				.With(x => x.isTransformLateMovement = true)
				.ReplaceWorldPosition(worldPosition)
				;

			return entity;
		}

		public static GameEntity SetupSmoothFollowMovement(
			this GameEntity entity,
			Vector3 worldPosition,
			Vector3 followOffset,
			float followSmoothSpeed)
		{
			entity.UnSetupAllMovements();

			entity
				.With(x => x.isSmoothFollowMovement = true)
				.ReplaceWorldPosition(worldPosition)
				.ReplaceFollowOffset(followOffset)
				.ReplaceFollowSmoothSpeed(followSmoothSpeed)
				;

			return entity;
		}
		
		public static Vector3 ClosestPointOnStep(this GameEntity mover, Vector3 point)
		{
			Vector3 from = mover.PreviousWorldPosition;
			Vector3 step = mover.WorldPosition - from;
			float stepLengthSquared = step.sqrMagnitude;

			if (stepLengthSquared <= Mathf.Epsilon)
				return from;

			float alongStep = Mathf.Clamp01(Vector3.Dot(point - from, step) / stepLengthSquared);

			return from + step * alongStep;
		}

		public static GameEntity UnSetupAllMovements(this GameEntity entity)
		{
			entity
				.With(x => x.isRigidbodyVelocityMovement = false)
				.With(x => x.isRigidbodyVelocity2DMovement = false)
				.With(x => x.isRigidbodyPositionMovement = false)
				.With(x => x.isRigidbodyInterpolatedMovement = false)
				.With(x => x.isSmoothFollowMovement = false)
				.With(x => x.isMoving = false)
				.SafeRemoveVelocity()
				.SafeRemoveMovementDirection()
				.SafeRemoveMovementStep()
				.SafeRemoveMovementSpeedMultiplier()
				.SafeRemoveRotationSpeedMultiplier()
				.SafeRemoveMovementSpeed()
				.SafeRemoveMinMovementSpeed()
				.SafeRemoveMovementSlowDownModifier()
				.SafeRemoveMovementSpeedUpModifier()
				.SafeRemoveFollowOffset()
				.SafeRemoveFollowSmoothSpeed()
				;

			if (entity.hasRigidbody)
				entity.Rigidbody.linearVelocity = Vector2.zero;

			if (entity.hasRigidbody2D)
				entity.Rigidbody2D.linearVelocity = Vector2.zero;

			return entity;
		}
	}
}