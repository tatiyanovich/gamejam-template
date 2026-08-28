using Code.Gameplay.Fuel.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Fuel
{
	public sealed class FuelFeature : Feature
	{
		public FuelFeature(ISystemFactory systems)
		{
			Add(systems.Create<InitializePlayerFuelSystem>());

			Add(systems.Create<DrainFuelWhileMovingSystem>());

			Add(systems.Create<MarkFuelEmptySystem>());
			Add(systems.Create<StopMovementWithoutFuelSystem>());
			Add(systems.Create<ForbidMovementWithoutFuelSystem>());
			Add(systems.Create<AllowMovementWithFuelSystem>());
		}
	}
}
