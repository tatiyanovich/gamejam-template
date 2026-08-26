using Code.Common.Cooldown.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Common.Cooldown
{
	public sealed class CooldownFeature : Feature
	{
		public CooldownFeature(ISystemFactory systems)
		{
			Add(systems.Create<CooldownSystem>());
		}
	}
}