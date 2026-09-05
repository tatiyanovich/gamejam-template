using Code.Infrastructure.Input;
using UnityEngine;

namespace Code.Editor
{
	public class PlaytestInput : IInputService
	{
		public KeyCode PressedKey { get; set; } = KeyCode.None;

		public bool IsKeyHeld(KeyCode key) => false;

		public bool IsKeyPressed(KeyCode key) => key == PressedKey;

		public Vector2 GetPointerScreenPosition() => Vector2.zero;
	}
}
