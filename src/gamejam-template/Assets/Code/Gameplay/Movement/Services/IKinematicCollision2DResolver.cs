using UnityEngine;

namespace Code.Gameplay.Movement.Services
{
	public interface IKinematicCollision2DResolver
	{
		void Resolve(Collider2D body, ContactFilter2D filter, int iterations, ref Vector2 position, ref Vector2 velocity);
	}
}
