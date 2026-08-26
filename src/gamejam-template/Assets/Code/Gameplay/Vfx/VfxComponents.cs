using Code.Gameplay.Vfx.Behaviours;
using Code.Gameplay.Vfx.Data;
using Entitas;

namespace Code.Gameplay.Vfx
{
	[Game] public class Vfx : IComponent { }
	[Game] public class VfxAnimatorComponent : IComponent { public VfxAnimator Value; }

	[Game] public class VfxTargetRadius : IComponent { public float Value; }

	[Game]
	public class PlayVfxRequest : IComponent
	{
		public VfxSpawnRequest Request;
	}
}
