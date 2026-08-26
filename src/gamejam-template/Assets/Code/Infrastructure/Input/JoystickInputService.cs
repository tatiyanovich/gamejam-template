using UnityEngine;

namespace Code.Infrastructure.Input
{
	public class JoystickInputService : IJoystickInputService
	{
		private Vector3 _joystickDirection;
		
		public void SetJoystickDirection(Vector3 direction)
		{
			_joystickDirection = direction;
		}

		public float GetHorizontalAxis()
		{
			if (_joystickDirection != Vector3.zero)
				return _joystickDirection.x;
			
			return UnityEngine.Input.GetAxisRaw("Horizontal");
		}		
		
		public float GetVerticalAxis()
		{
			if (_joystickDirection != Vector3.zero)
				return _joystickDirection.y;
			
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