using UnityEngine;

namespace Code.Infrastructure.Input
{
	public interface IInputService
	{
		float GetHorizontalAxis();
		bool IsRestartInputStarted();
		float GetVerticalAxis();
		Vector2 GetPointerScreenPosition();
	}
}