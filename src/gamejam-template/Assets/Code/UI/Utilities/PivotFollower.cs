using Code.Gameplay.Camera.Services;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Utilities
{
	public class PivotFollower : MonoBehaviour
	{
		[SF] private RectTransform anchoredRect;
		[SF] private float worldHeightOffset;

		private ICameraQuery _cameraQuery;

		private Transform _pivot;
		private Canvas _canvas;
		private RectTransform _parentRect;

		[Inject]
		private void Construct(ICameraQuery cameraQuery)
		{
			_cameraQuery = cameraQuery;
		}

		public void Follow(Transform pivot)
		{
			_canvas = anchoredRect.GetComponentInParent<Canvas>();
			_parentRect = (RectTransform)anchoredRect.parent;
			_pivot = pivot;

			UpdatePosition();
		}

		public void StopFollowing()
		{
			_pivot = null;
		}

		private void LateUpdate()
		{
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			if (_pivot == null)
				return;

			Camera camera = _cameraQuery.GetCamera();

			if (camera == null)
				return;

			Vector3 worldPosition = _pivot.position + Vector3.up * worldHeightOffset;
			Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);

			if (screenPosition.z <= 0f)
				return;

			Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
				? null
				: _canvas.worldCamera;

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				_parentRect, screenPosition, uiCamera, out Vector2 anchored))
			{
				anchoredRect.anchoredPosition = anchored;
			}
		}
	}
}
