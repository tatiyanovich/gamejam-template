using Entitas;
using UnityEngine;

namespace Code.Gameplay.Player
{
	[Game] public class Player : IComponent { }
	[Game] public class SpawnPoint : IComponent { public Transform Value; }

	[Game] public class BodyCollider2D : IComponent { public Collider2D Value; }
}
