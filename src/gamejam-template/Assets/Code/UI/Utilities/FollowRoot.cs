using Code.Gameplay.Camera.Services;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Utilities
{
	[RequireComponent(typeof(RectTransform))]
	public class FollowRoot : MonoBehaviour
	{
		[SF] private float worldHeightOffset;

		private ICameraQuery _cameraQuery;

		private RectTransform _rect;
		private Transform _pivot;
		private Vector3 _worldPosition;
		private bool _useWorldPosition;
		private Canvas _canvas;
		private RectTransform _parentRect;
		private Vector2 _offset;

		[Inject]
		private void Construct(ICameraQuery cameraQuery)
		{
			_cameraQuery = cameraQuery;
		}

		private void Awake()
		{
			_rect = (RectTransform)transform;
		}

		public void Follow(Transform pivot)
		{
			_pivot = pivot;
			_useWorldPosition = false;
			_canvas = _rect.GetComponentInParent<Canvas>();
			_parentRect = (RectTransform)_rect.parent;

			UpdatePosition();
		}

		public void Follow(Vector3 worldPosition)
		{
			_pivot = null;
			_useWorldPosition = true;
			_worldPosition = worldPosition;
			_canvas = _rect.GetComponentInParent<Canvas>();
			_parentRect = (RectTransform)_rect.parent;

			UpdatePosition();
		}

		public void StopFollowing()
		{
			_pivot = null;
			_useWorldPosition = false;
			_offset = Vector2.zero;
		}

		public void SetOffset(Vector2 offset)
		{
			_offset = offset;
		}

		private void LateUpdate()
		{
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			if (_useWorldPosition == false && _pivot == null)
				return;

			Camera mainCamera = _cameraQuery.GetCamera();

			if (mainCamera == null)
				return;

			Vector3 pivotPosition = _useWorldPosition ? _worldPosition : _pivot.position;
			Vector3 worldPosition = pivotPosition + Vector3.up * worldHeightOffset;
			Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

			if (screenPosition.z <= 0f)
				return;

			Camera uiCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay
				? null
				: _canvas.worldCamera;

			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
				    _parentRect, screenPosition, uiCamera, out Vector2 anchored))
			{
				_rect.anchoredPosition = anchored + _offset;
			}
		}
	}
}
