using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class IntegrateKinematicVelocitySystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _movers;

		public IntegrateKinematicVelocitySystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.KinematicMovement,
					GameMatcher.WorldPosition,
					GameMatcher.Velocity,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			float deltaTime = _timeService.DeltaTime;

			foreach (GameEntity mover in _movers)
			{
				Vector3 previous = mover.WorldPosition;

				mover.ReplacePreviousWorldPosition(previous);
				mover.ReplaceWorldPosition(previous + mover.Velocity * deltaTime);
			}
		}
	}
}
