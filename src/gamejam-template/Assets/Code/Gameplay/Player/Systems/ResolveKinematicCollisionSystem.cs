using System.Collections.Generic;
using Code.Gameplay.Movement.Services;
using Code.Gameplay.Player.Services;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player.Systems
{
	public sealed class ResolveKinematicCollisionSystem : IExecuteSystem
	{
		private const int ResolveIterations = 3;

		private readonly IKinematicCollision2DResolver _collisionResolver;

		private readonly IGroup<GameEntity> _bodies;

		private readonly List<GameEntity> _buffer = new(4);
		private readonly ContactFilter2D _filter;

		public ResolveKinematicCollisionSystem(
			GameContext game,
			IPlayerConfigsService configsService,
			IKinematicCollision2DResolver collisionResolver)
		{
			_collisionResolver = collisionResolver;

			_filter = new ContactFilter2D
			{
				useTriggers = false,
				useLayerMask = true,
				layerMask = configsService.PlayerConfig.ObstacleMask
			};

			_bodies = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.KinematicMovement,
					GameMatcher.WorldPosition,
					GameMatcher.Velocity,
					GameMatcher.BodyCollider2D));
		}

		public void Execute()
		{
			foreach (GameEntity body in _bodies.GetEntities(_buffer))
			{
				Vector3 world = body.WorldPosition;
				Vector2 position = world;
				Vector2 velocity = body.Velocity;

				_collisionResolver.Resolve(body.BodyCollider2D, _filter, ResolveIterations, ref position, ref velocity);

				body.ReplaceWorldPosition(new Vector3(position.x, position.y, world.z));
				body.ReplaceVelocity(new Vector3(velocity.x, velocity.y, 0f));
			}
		}
	}
}
