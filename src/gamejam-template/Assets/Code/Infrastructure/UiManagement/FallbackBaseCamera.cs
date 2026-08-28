using Code.Infrastructure.UiManagement.Services;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.Infrastructure.UiManagement
{
	public class FallbackBaseCamera : MonoBehaviour
	{
		[SF] private Camera uiCamera;

		private GameObject _fallbackCamera;
		private bool _isQuitting;

		private IFallbackBaseCameraService _fallbackBaseCameraService;

		[Inject]
		private void Construct(IFallbackBaseCameraService fallbackBaseCameraService)
		{
			_fallbackBaseCameraService = fallbackBaseCameraService;
		}

		private void Awake()
		{
			_fallbackBaseCameraService.Register(this);
		}

		private void OnApplicationQuit()
		{
			_isQuitting = true;
		}

		public void EnableCamera()
		{
			if (_isQuitting)
				return;

			if (_fallbackCamera == null)
			{
				CreateCamera();
				return;
			}

			_fallbackCamera.SetActive(true);
		}

		public void DisableCamera()
		{
			if (_fallbackCamera != null)
				_fallbackCamera.SetActive(false);
		}

		private void CreateCamera()
		{
			_fallbackCamera = new GameObject("FallbackBaseCamera");
			_fallbackCamera.transform.SetParent(transform);

			Camera baseCamera = _fallbackCamera.AddComponent<Camera>();
			baseCamera.clearFlags = CameraClearFlags.SolidColor;
			baseCamera.backgroundColor = new Color(10f / 255f, 10f / 255f, 10f / 255f);
			baseCamera.cullingMask = 0;
			baseCamera.depth = -100;

			UniversalAdditionalCameraData cameraData = baseCamera.GetUniversalAdditionalCameraData();
			cameraData.renderType = CameraRenderType.Base;
			cameraData.cameraStack.Add(uiCamera);
		}
	}
}
