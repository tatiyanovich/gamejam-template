using Code.Gameplay.Neighbours.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Neighbours
{
	public sealed class NeighboursFeature : Feature
	{
		public NeighboursFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeNeighboursSystem>());

			Add(systemFactory.Create<LiftPawOnMeowSystem>());
			Add(systemFactory.Create<CoverPawOnTimerSystem>());
		}
	}
}
