using Entitas;

namespace Code.Gameplay.Meow
{
	[Game] public class MeowSource : IComponent { }
	[Game] public class MicrophoneLevel : IComponent { public float Value; }
	[Game] public class MeowArmed : IComponent { }

	[Game] public class MeowEvent : IComponent { }
}
