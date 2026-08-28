using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Drilling
{
	[Game] public class DrillRun : IComponent { }
	[Game, Watched] public class DrilledDistance : IComponent { public float Value; }
	[Game, Watched] public class BestDrilledDistance : IComponent { public float Value; }

	[Game, Watched] public class RunFinished : IComponent { }
}
