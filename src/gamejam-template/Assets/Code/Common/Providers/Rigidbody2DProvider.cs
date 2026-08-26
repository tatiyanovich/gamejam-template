using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Common.Providers
{
	public class Rigidbody2DProvider : EntityComponentProvider
	{
		[SF] private Rigidbody2D rigidbody2D;

		public override void RegisterComponents() => Entity.AddRigidbody2D(rigidbody2D);
		public override void UnregisterComponents() => Entity.SafeRemoveRigidbody2D();
	}
}
