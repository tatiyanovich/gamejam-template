using Entitas;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public class RigidbodyInterpolationMovementSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public RigidbodyInterpolationMovementSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RigidbodyInterpolatedMovement,
					GameMatcher.MovementDirection,
					GameMatcher.MovementSpeed,
					GameMatcher.Velocity,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				float movementInterpolation = 1;

				if (mover.MovementDirection != Vector3.zero)
				{
					movementInterpolation = mover.MovementSpeedUpModifier;
				}
				else
				{
					movementInterpolation = mover.MovementSlowDownModifier;
				}

				Vector3 velocity = Vector3.Lerp(
					mover.Velocity,
					mover.MovementDirection * mover.MovementSpeed,
					movementInterpolation);

				mover.ReplaceVelocity(velocity);
			}
		}
	}
}
