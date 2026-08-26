using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Common.Cooldown
{
	[Game, Watched] public class OnCooldown : IComponent { }
	[Game] public class CooldownTimeLeft : IComponent { public float Value; }
}