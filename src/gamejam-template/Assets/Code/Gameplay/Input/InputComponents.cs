using Code.Gameplay.Exam;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace Code.Gameplay.Input
{
	[Input] public class Input : IComponent { }
	[Input] public class PointerWorldPosition : IComponent { public Vector3 Value; }
	[Input, Watched] public class LeanHeld : IComponent { }
	[Input] public class StrokeInput : IComponent { public StrokeDirection Value; }
	[Input] public class PickInput : IComponent { public int Value; }
	[Input] public class LetterInput : IComponent { public char Value; }
	[Input] public class MeowKeyPressed : IComponent { }
	[Input] public class DuckKeyPressed : IComponent { }
}
