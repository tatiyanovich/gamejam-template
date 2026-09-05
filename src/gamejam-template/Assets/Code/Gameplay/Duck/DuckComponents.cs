using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Duck
{
	[Game] public class Duck : IComponent { }
	[Game, Watched] public class DuckStateComponent : IComponent { public DuckState Value; }
	[Game] public class DuckStateTimeLeft : IComponent { public float Value; }
	[Game, Watched] public class DuckThrowCount : IComponent { public int Value; }

	[Game] public class ThrowDuckRequest : IComponent { }

	[Game] public class DuckThrownEvent : IComponent { }
}
