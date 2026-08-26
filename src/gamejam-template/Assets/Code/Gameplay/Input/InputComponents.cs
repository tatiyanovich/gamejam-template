using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input
{
	[Input] public class Input : IComponent { }
	[Input] public class HorizontalAxis : IComponent { public float Value; }
	[Input] public class VerticalAxis : IComponent { public float Value; }
	[Input] public class PointerWorldPosition : IComponent { public Vector3 Value; }
}