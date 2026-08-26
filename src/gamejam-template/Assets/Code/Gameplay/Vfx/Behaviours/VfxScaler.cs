using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Vfx.Behaviours
{
	public class VfxScaler : EntityComponentProvider
	{
		[SF, Min(0.01f)] private float unitRadius = 1f;

		public override void RegisterComponents()
		{
			if (Entity.hasVfxTargetRadius == false)
				return;

			Entity.ReplaceLossyScale(Vector3.one * (Entity.VfxTargetRadius / unitRadius));
		}

		public override void UnregisterComponents()
		{
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.position, unitRadius);
		}
	}
}
