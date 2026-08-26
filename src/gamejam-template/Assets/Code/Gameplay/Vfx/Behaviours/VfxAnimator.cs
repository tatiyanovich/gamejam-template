using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Vfx.Behaviours
{
	public class VfxAnimator : EntityComponentProvider
	{
		[SF] private ParticleSystem particle;

		public override void RegisterComponents() => Entity.AddVfxAnimator(this);
		public override void UnregisterComponents() => Entity.SafeRemoveVfxAnimator();

		private void OnEnable()
		{
			particle.Play(withChildren: true);
		}

		public bool IsPlaying()
		{
			return particle.IsAlive(withChildren: true);
		}
	}
}
