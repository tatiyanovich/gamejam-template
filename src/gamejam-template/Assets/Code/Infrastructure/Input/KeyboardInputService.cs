using UnityEngine;

namespace Code.Infrastructure.Input
{
	public class KeyboardInputService : IInputService
	{
		public bool IsKeyHeld(KeyCode key)
		{
			return UnityEngine.Input.GetKey(key);
		}

		public bool IsKeyPressed(KeyCode key)
		{
			return UnityEngine.Input.GetKeyDown(key);
		}

		public Vector2 GetPointerScreenPosition()
		{
			return UnityEngine.Input.mousePosition;
		}
	}
}
