using System.Collections.Generic;
using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public class RotateTowardsLookDirectionSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;
		private readonly ITimeService _timeService;
		
		private readonly List<GameEntity> _moversBuffer = new(16);

		public RotateTowardsLookDirectionSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.WorldRotation,
					GameMatcher.LookDirection,
					GameMatcher.RotationSpeed,
					GameMatcher.Alive));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers.GetEntities(_moversBuffer))
			{
				Vector3 lookDirection = mover.LookDirection;
				lookDirection.y = 0f;

				if (lookDirection.sqrMagnitude > 0f)
				{
					Quaternion targetRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
					Quaternion worldRotation = Quaternion.Slerp(
						mover.WorldRotation,
						targetRotation,
						mover.RotationSpeed * _timeService.DeltaTime);

					mover.ReplaceWorldRotation(worldRotation);
				}
			}
		}
	}
}
