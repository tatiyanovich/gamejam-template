using Code.Gameplay.Drilling;
using Code.Gameplay.Fuel;
using Code.Gameplay.Movement;
using Code.Gameplay.Player;
using Code.Gameplay.Teardown;
using Code.Gameplay.UI;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.SceneEntities.Systems;

namespace Code.Gameplay
{
	public sealed class GameplayCoreFeature : Feature
	{
		public GameplayCoreFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeSceneEntitiesByRequestSystem>());

			Add(systemFactory.Create<PlayerFeature>());

			Add(systemFactory.Create<MovementUpdateFeature>());

			Add(systemFactory.Create<FuelFeature>());
			Add(systemFactory.Create<DrillingFeature>());

			Add(systemFactory.Create<UIFeature>());

			Add(systemFactory.Create<GameplayTeardownSystem>());
		}
	}
}
