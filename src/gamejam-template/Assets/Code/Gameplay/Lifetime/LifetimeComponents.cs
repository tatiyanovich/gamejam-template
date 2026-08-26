using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Lifetime
{
	[Game, Watched] public class CurrentHP : IComponent { public float Value; }
	[Game] public class MaxHP : IComponent { public float Value; }

	[Game] public class LifetimeLeft : IComponent { public float Value; }
	[Game] public class Dead : IComponent { }
	[Game, Watched] public class Alive : IComponent { }
	[Game] public class Damaged : IComponent { }
	[Game] public class Damageable : IComponent { }

	[Game] public class DestructOnDeath : IComponent { }
	[Game] public class HideOnDeath : IComponent { }

	[Game]
	public class DeathEvent : IComponent
	{
		public int DeadEntityId;
	}

	[Game]
	public class DamageEvent : IComponent
	{
		public int TargetEntityId;
		public float Amount;
		public Vector3 Position;
	}
}
