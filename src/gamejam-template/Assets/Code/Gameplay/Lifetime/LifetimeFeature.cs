using Code.Gameplay.Lifetime.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Lifetime
{
	public sealed class LifetimeFeature : Feature
	{
		public LifetimeFeature(ISystemFactory systems)
		{
			Add(systems.Create<MarkLifeStateSystem>());
			Add(systems.Create<MarkDamageableSystem>());
			Add(systems.Create<MarkDamagedSystem>());
			Add(systems.Create<LifetimeCounterSystem>());

			Add(systems.Create<DestructOnLifetimeOverSystem>());
			Add(systems.Create<DestructOnDeathSystem>());

			Add(systems.Create<HideOnDeathSystem>());
		}
	}
}
