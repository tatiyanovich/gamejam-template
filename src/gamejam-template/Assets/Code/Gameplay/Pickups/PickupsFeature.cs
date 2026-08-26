using Code.Gameplay.Pickups.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Pickups
{
	public sealed class PickupsFeature : Feature
	{
		public PickupsFeature(ISystemFactory systems)
		{
			Add(systems.Create<InitializeScoreSystem>());
			Add(systems.Create<InitializePickupsSystem>());

			Add(systems.Create<CollectPickupsSystem>());
			Add(systems.Create<AccumulateScoreSystem>());
		}
	}
}
