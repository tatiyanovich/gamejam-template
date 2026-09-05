using System.Collections.Generic;
using Code.Gameplay.Exam;
using UnityEngine;

namespace Code.Gameplay.Input.Data
{
	public static class InputKeyMap
	{
		public const KeyCode Lean = KeyCode.Space;
		public const KeyCode Meow = KeyCode.M;
		public const KeyCode Duck = KeyCode.Q;

		public static readonly KeyBinding<StrokeDirection>[] Strokes =
		{
			new(KeyCode.UpArrow, StrokeDirection.Up),
			new(KeyCode.W, StrokeDirection.Up),
			new(KeyCode.RightArrow, StrokeDirection.Right),
			new(KeyCode.D, StrokeDirection.Right),
			new(KeyCode.DownArrow, StrokeDirection.Down),
			new(KeyCode.S, StrokeDirection.Down),
			new(KeyCode.LeftArrow, StrokeDirection.Left),
			new(KeyCode.A, StrokeDirection.Left)
		};

		public static readonly KeyBinding<int>[] Picks =
		{
			new(KeyCode.Alpha1, 0),
			new(KeyCode.Alpha2, 1),
			new(KeyCode.Alpha3, 2),
			new(KeyCode.Alpha4, 3)
		};

		public static readonly KeyBinding<char>[] Letters = BuildLetters();

		private static KeyBinding<char>[] BuildLetters()
		{
			List<KeyBinding<char>> letters = new(24);

			for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
			{
				if (key == Meow || key == Duck)
					continue;

				letters.Add(new KeyBinding<char>(key, (char)('A' + (key - KeyCode.A))));
			}

			return letters.ToArray();
		}
	}
}
