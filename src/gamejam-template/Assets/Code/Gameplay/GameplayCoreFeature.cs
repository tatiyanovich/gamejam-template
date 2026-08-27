using Code.Gameplay.Fuel;
using Code.Gameplay.Movement;
using Code.Gameplay.Pickups;
using Code.Gameplay.Player;
using Code.Gameplay.Teardown;
using Code.Gameplay.UI;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.SceneEntities.Systems;

namespace Code.Gameplay
{
	// The gameplay node's pipeline. Infrastructure runs around it (GlobalLoopInfraHeadFeature /
	// GlobalLoopInfraTailFeature), so only add gameplay features here, in execution order.
	public sealed class GameplayCoreFeature : Feature
	{
		public GameplayCoreFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InitializeSceneEntitiesByRequestSystem>());

			Add(systemFactory.Create<PlayerFeature>());
			Add(systemFactory.Create<PickupsFeature>());

			Add(systemFactory.Create<MovementUpdateFeature>());

			Add(systemFactory.Create<FuelFeature>());

			Add(systemFactory.Create<UIFeature>());

			Add(systemFactory.Create<GameplayTeardownSystem>());
		}
	}
}
