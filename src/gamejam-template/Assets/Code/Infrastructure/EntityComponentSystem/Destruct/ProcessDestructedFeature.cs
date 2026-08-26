using Code.Infrastructure.EntityComponentSystem.Destruct.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Infrastructure.EntityComponentSystem.Destruct
{
	public sealed class ProcessDestructedFeature : Feature
	{
		public ProcessDestructedFeature(ISystemFactory systems)
		{
			Add(systems.Create<CleanupGameDestructedViewSystem>());
			Add(systems.Create<CleanupGameDestructedSystem>());
		}
	}
}
