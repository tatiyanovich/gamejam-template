using UnityEngine;

namespace Code.Infrastructure.Input
{
	public class KeyboardInputService : IInputService
	{
		public float GetHorizontalAxis()
		{
			return UnityEngine.Input.GetAxisRaw("Horizontal");
		}

		public float GetVerticalAxis()
		{
			return UnityEngine.Input.GetAxisRaw("Vertical");
		}

		public bool IsRestartInputStarted()
		{
			return UnityEngine.Input.GetKeyDown(KeyCode.R);
		}

		public Vector2 GetPointerScreenPosition()
		{
			return UnityEngine.Input.mousePosition;
		}
	}
}
