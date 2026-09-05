using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Neighbours
{
	[Game] public class Neighbour : IComponent { }
	[Game] public class NeighbourSideComponent : IComponent { public NeighbourSide Value; }
	[Game, Watched] public class PawLifted : IComponent { }
	[Game] public class PawWindowTimeLeft : IComponent { public float Value; }
}
