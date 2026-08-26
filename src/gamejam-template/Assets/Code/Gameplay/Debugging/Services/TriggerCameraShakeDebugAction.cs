using Code.Gameplay.Camera;
using Code.Gameplay.Camera.Services;
using UnityEngine;

namespace Code.Gameplay.Debugging.Services
{
	public class TriggerCameraShakeDebugAction : IGameplayDebugInputAction
	{
		private readonly ICameraFactory _cameraFactory;

		public TriggerCameraShakeDebugAction(ICameraFactory cameraFactory)
		{
			_cameraFactory = cameraFactory;
		}

		public bool WasTriggeredThisFrame()
		{
			return UnityEngine.Input.GetKeyDown(KeyCode.Space)
				|| UnityEngine.Input.GetMouseButtonDown(2);
		}

		public void Execute(Vector3 pointerWorldPosition)
		{
			_cameraFactory.CreateShakeRequest(CameraShakeTypeId.Default);
		}
	}
}
