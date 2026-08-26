using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class ComputeMovementStepSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;
		private readonly IGroup<GameEntity> _movers;

		public ComputeMovementStepSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.TransformMovement,
					GameMatcher.MovementDirection,
					GameMatcher.MovementSpeed,
					GameMatcher.MinMovementSpeed,
					GameMatcher.MovementSpeedMultiplier,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				float speed = Mathf.Max(
					mover.MovementSpeed * mover.MovementSpeedMultiplier,
					mover.MinMovementSpeed);

				Vector3 step = mover.MovementDirection * speed * _timeService.DeltaTime;

				mover.ReplaceMovementStep(step);
			}
		}
	}
}
