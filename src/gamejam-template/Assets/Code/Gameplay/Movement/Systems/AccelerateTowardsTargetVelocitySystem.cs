using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class AccelerateTowardsTargetVelocitySystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _movers;

		public AccelerateTowardsTargetVelocitySystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.KinematicMovement,
					GameMatcher.Velocity,
					GameMatcher.TargetVelocity,
					GameMatcher.MovementAcceleration,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			float deltaTime = _timeService.DeltaTime;

			foreach (GameEntity mover in _movers)
			{
				float smoothing = 1f - Mathf.Exp(-mover.MovementAcceleration * deltaTime);

				Vector3 velocity = Vector3.Lerp(
					mover.Velocity,
					mover.TargetVelocity,
					smoothing);

				mover.ReplaceVelocity(velocity);
			}
		}
	}
}
