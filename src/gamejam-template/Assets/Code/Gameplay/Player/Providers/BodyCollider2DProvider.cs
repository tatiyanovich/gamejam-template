using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Player.Providers
{
	public class BodyCollider2DProvider : EntityComponentProvider
	{
		[SF] private Collider2D bodyCollider;

		public override void RegisterComponents() => Entity.AddBodyCollider2D(bodyCollider);
		public override void UnregisterComponents() => Entity.SafeRemoveBodyCollider2D();
	}
}
