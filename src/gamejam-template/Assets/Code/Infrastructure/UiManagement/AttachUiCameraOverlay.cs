using System;
using Code.Infrastructure.UiManagement.Services;
using Framework.UI.UiManagement.Services;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

namespace Code.Infrastructure.UiManagement
{
	[RequireComponent(typeof(Camera))]
	public class AttachUiCameraOverlay : MonoBehaviour
	{
		private IUiHolder _uiHolder;
		private IFallbackBaseCameraService _fallbackBaseCameraService;
		private Camera _camera;

		[Inject]
		private void Construct(IUiHolder uiHolder, IFallbackBaseCameraService fallbackBaseCameraService)
		{
			_uiHolder = uiHolder;
			_fallbackBaseCameraService = fallbackBaseCameraService;
			_camera = GetComponent<Camera>();
		}

		private void Awake()
		{
			UniversalAdditionalCameraData cameraData = _camera.GetUniversalAdditionalCameraData();

			if (cameraData.cameraStack.Contains(_uiHolder.UiCamera) == false)
			{
				cameraData.cameraStack.Add(_uiHolder.UiCamera);
			}

			// This world camera is now the base camera, so retire the loading fallback — otherwise the
			// UI overlay would render through two base cameras.
			_fallbackBaseCameraService.DisableCamera();
		}

		private void OnDestroy()
		{
			_fallbackBaseCameraService.EnableCamera();
		}
	}
}