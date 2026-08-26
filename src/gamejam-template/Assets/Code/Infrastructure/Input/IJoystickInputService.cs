using UnityEngine;

namespace Code.Infrastructure.Input
{
	public interface IJoystickInputService : IInputService
	{
		void SetJoystickDirection(Vector3 direction);
	}
}