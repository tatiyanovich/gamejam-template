using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Parent
{
	[Game] public class Parent : IComponent { public Transform Value; }
	[Game] public class ParentId : IComponent { public int Value; }
	[Game] public class ParentAttached : IComponent {  }
}