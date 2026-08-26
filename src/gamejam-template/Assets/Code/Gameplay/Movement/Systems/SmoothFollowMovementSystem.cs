using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public class SmoothFollowMovementSystem : IExecuteSystem
	{
		private readonly GameContext _game;
		private readonly ITimeService _timeService;
		private readonly IGroup<GameEntity> _followers;

		public SmoothFollowMovementSystem(GameContext game, ITimeService timeService)
		{
			_game = game;
			_timeService = timeService;

			_followers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.SmoothFollowMovement,
					GameMatcher.TargetId,
					GameMatcher.WorldPosition,
					GameMatcher.FollowOffset,
					GameMatcher.FollowSmoothSpeed));
		}

		public void Execute()
		{
			foreach (GameEntity follower in _followers)
			{
				GameEntity target = _game.GetEntityWithId(follower.TargetId);

				if (target == null)
					continue;

				Vector3 desiredPosition = target.WorldPosition + follower.FollowOffset;

				float smoothFactor = 1f - Mathf.Exp(-follower.FollowSmoothSpeed * _timeService.DeltaTime);

				Vector3 smoothedPosition = Vector3.Lerp(
					follower.WorldPosition,
					desiredPosition,
					smoothFactor);

				follower.ReplaceWorldPosition(smoothedPosition);
			}
		}
	}
}
