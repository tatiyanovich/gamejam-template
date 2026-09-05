using UnityEngine;

namespace Code.Infrastructure.Input
{
	public interface IInputService
	{
		bool IsKeyHeld(KeyCode key);
		bool IsKeyPressed(KeyCode key);
		Vector2 GetPointerScreenPosition();
	}
}
