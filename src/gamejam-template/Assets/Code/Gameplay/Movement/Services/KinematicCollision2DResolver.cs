using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Movement.Services
{
	public sealed class KinematicCollision2DResolver : IKinematicCollision2DResolver
	{
		private const float MinPushSqrMagnitude = 0.0000001f;

		private readonly List<Collider2D> _overlap = new(16);

		public void Resolve(Collider2D body, ContactFilter2D filter, int iterations, ref Vector2 position, ref Vector2 velocity)
		{
			Transform transform = body.transform;
			Rigidbody2D bodyRigidbody = body.attachedRigidbody;
			float z = transform.position.z;

			for (int iteration = 0; iteration < iterations; iteration++)
			{
				transform.position = new Vector3(position.x, position.y, z);
				Physics2D.SyncTransforms();

				int count = body.Overlap(filter, _overlap);
				Vector2 totalPush = Vector2.zero;

				for (int i = 0; i < count; i++)
				{
					Collider2D collider = _overlap[i];

					if (collider == body)
						continue;

					if (bodyRigidbody != null && collider.attachedRigidbody == bodyRigidbody)
						continue;

					ColliderDistance2D separation = body.Distance(collider);

					if (separation.isValid == false || separation.distance >= 0f)
						continue;

					Vector2 push = separation.normal * separation.distance;

					if (push.sqrMagnitude <= MinPushSqrMagnitude)
						continue;

					totalPush += push;

					Vector2 outward = push.normalized;
					float into = Vector2.Dot(velocity, outward);

					if (into < 0f)
						velocity -= outward * into;
				}

				if (totalPush.sqrMagnitude <= MinPushSqrMagnitude)
					break;

				position += totalPush;
			}
		}
	}
}
