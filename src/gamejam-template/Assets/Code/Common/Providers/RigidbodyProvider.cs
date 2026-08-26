using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Common.Providers
{
	public class RigidbodyProvider : EntityComponentProvider
	{
		[SF] private new Rigidbody rigidbody;

		public override void RegisterComponents() => Entity.AddRigidbody(rigidbody);
		public override void UnregisterComponents() => Entity.SafeRemoveRigidbody();
	}
}