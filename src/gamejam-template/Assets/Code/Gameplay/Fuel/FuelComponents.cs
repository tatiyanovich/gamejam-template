using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Fuel
{
	[Game, Watched] public class Fuel : IComponent { public float Value; }
	[Game] public class MaxFuel : IComponent { public float Value; }
	[Game] public class FuelDrainRate : IComponent { public float Value; }

	[Game] public class FuelEmpty : IComponent { }
}
