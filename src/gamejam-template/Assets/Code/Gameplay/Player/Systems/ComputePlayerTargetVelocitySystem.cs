using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
	public sealed class ComputePlayerTargetVelocitySystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _players;

		public ComputePlayerTargetVelocitySystem(GameContext game)
		{
			_players = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.KinematicMovement,
					GameMatcher.MovementDirection,
					GameMatcher.MovementSpeed,
					GameMatcher.MaxMovementSpeed,
					GameMatcher.MovementSpeedMultiplier,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			foreach (GameEntity player in _players)
			{
				float speed = Mathf.Min(
					player.MovementSpeed * player.MovementSpeedMultiplier,
					player.MaxMovementSpeed);

				player.ReplaceTargetVelocity(player.MovementDirection * speed);
			}
		}
	}
}
