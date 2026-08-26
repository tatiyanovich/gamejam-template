using Code.Gameplay.Vfx.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Vfx
{
	public sealed class VfxFeature : Feature
	{
		public VfxFeature(ISystemFactory systems)
		{
			Add(systems.Create<PlayVfxByRequestSystem>());
			Add(systems.Create<DestructCompletedVfxSystem>());
		}
	}
}
