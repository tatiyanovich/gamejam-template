using Code.Gameplay.Drilling.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Drilling
{
	public sealed class DrillingFeature : Feature
	{
		public DrillingFeature(ISystemFactory systems)
		{
			Add(systems.Create<InitializeDrillRunSystem>());

			Add(systems.Create<AccumulateDrilledDistanceSystem>());
			Add(systems.Create<TrackBestDrilledDistanceSystem>());

			Add(systems.Create<FinishRunOnEmptyFuelSystem>());
		}
	}
}
