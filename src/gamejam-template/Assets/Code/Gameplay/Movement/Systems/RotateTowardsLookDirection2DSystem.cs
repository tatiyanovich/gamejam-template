using System.Collections.Generic;
using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	/// <summary>
	/// Top-down 2D facing: rotates WorldRotation around the Z axis so the sprite's local up (+Y) points
	/// along its LookDirection in the XY plane. Sprites are authored facing up — the top-down art
	/// convention. The 3D RotateTowardsLookDirectionSystem rotates around Y and is not usable for sprites.
	/// </summary>
	public sealed class RotateTowardsLookDirection2DSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _rotators;
		private readonly ITimeService _timeService;

		private readonly List<GameEntity> _buffer = new(16);

		public RotateTowardsLookDirection2DSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_rotators = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.WorldRotation,
					GameMatcher.LookDirection,
					GameMatcher.RotationSpeed));
		}

		public void Execute()
		{
			foreach (GameEntity rotator in _rotators.GetEntities(_buffer))
			{
				Vector3 lookDirection = rotator.LookDirection;
				lookDirection.z = 0f;

				if (lookDirection.sqrMagnitude <= 0f)
					continue;

				// atan2 measures from +X; sprites are drawn facing +Y, so subtract 90 to align the
				// sprite's up with the look direction. Always rotates around Z, keeping it in-plane.
				float targetAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg - 90f;
				Quaternion targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);

				Quaternion worldRotation = Quaternion.RotateTowards(
					rotator.WorldRotation,
					targetRotation,
					rotator.RotationSpeed * _timeService.DeltaTime);

				rotator.ReplaceWorldRotation(worldRotation);
			}
		}
	}
}
