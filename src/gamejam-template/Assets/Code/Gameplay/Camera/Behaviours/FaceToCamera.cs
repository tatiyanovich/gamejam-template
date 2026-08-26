using Code.Gameplay.Camera.Services;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Camera.Behaviours
{
	public class FaceToCamera : MonoBehaviour
	{
		[SF] private bool invertForward = true;
		
		private ICameraQuery _cameraQuery;

		[Inject]
		private void Construct(ICameraQuery cameraQuery)
		{
			_cameraQuery = cameraQuery;
		}

		private void LateUpdate()
		{
			UnityEngine.Camera mainCamera = _cameraQuery.GetCamera();
			
			if(mainCamera == null)
				return;
			
			Quaternion camRot = mainCamera.transform.rotation;

			if (invertForward)
				camRot *= Quaternion.Euler(0f, 180f, 0f);

			transform.rotation = camRot;
		}
	}
}