using UnityEngine;

namespace Code.Gameplay.Debugging.Services
{
	public interface IGameplayDebugInputAction
	{
		bool WasTriggeredThisFrame();
		void Execute(Vector3 pointerWorldPosition);
	}
}
