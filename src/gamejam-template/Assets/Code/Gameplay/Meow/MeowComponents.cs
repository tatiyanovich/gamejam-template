using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Meow
{
	[Game] public class MeowSource : IComponent { }
	[Game, Watched] public class MicrophoneLevel : IComponent { public float Value; }
	[Game] public class MeowArmed : IComponent { }

	[Game] public class MeowEvent : IComponent { }
}
