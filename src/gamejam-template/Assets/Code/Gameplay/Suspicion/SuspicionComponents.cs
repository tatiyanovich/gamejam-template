using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Suspicion
{
	[Game, Watched] public class SuspicionLevel : IComponent { public float Value; }
}
