using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class SyncTransformFromRigidbody2DSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;
		private readonly List<GameEntity> _buffer = new(16);

		public SyncTransformFromRigidbody2DSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RigidbodyVelocity2DMovement,
					GameMatcher.Rigidbody2D,
					GameMatcher.Transform,
					GameMatcher.WorldPosition));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				Transform transform = mover.Transform;

				Vector3 position = transform.position;
				Vector3 worldPosition = new(position.x, position.y, mover.WorldPosition.z);

				mover.ReplacePreviousWorldPosition(mover.WorldPosition);
				mover.ReplaceWorldPosition(worldPosition);

				if (mover.hasWorldRotation)
					mover.ReplaceWorldRotation(transform.rotation);
			}
		}
	}
}
