using Code.Gameplay.Fuel.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Fuel
{
	// Runs after MovementUpdateFeature in GameplayCoreFeature so the Moving flag it drains on
	// was set this frame, not the previous one.
	public sealed class FuelFeature : Feature
	{
		public FuelFeature(ISystemFactory systems)
		{
			Add(systems.Create<InitializePlayerFuelSystem>());

			Add(systems.Create<RefuelOnPickupCollectedSystem>());
			Add(systems.Create<DrainFuelWhileMovingSystem>());

			Add(systems.Create<MarkFuelEmptySystem>());
			Add(systems.Create<StopMovementWithoutFuelSystem>());
			Add(systems.Create<ForbidMovementWithoutFuelSystem>());
			Add(systems.Create<AllowMovementWithFuelSystem>());
		}
	}
}
