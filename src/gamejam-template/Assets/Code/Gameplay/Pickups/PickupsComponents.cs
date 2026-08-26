using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Pickups
{
	[Game] public class Pickup : IComponent { }
	[Game] public class ScoreValue : IComponent { public int Value; }

	[Game] public class ScoreHolder : IComponent { }
	[Game, Watched] public class Score : IComponent { public int Value; }

	[Game] public class CollectRadius : IComponent { public float Value; }

	[Game] public class PickupCollectedEvent : IComponent { public int Amount; }
}
