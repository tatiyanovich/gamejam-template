using Code.Infrastructure.Input;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Joystick
{
	public class JoystickWidget : WidgetBase, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		[SF] private float movementRange = 50;
		[SF] private RectTransform handle;
		[SF] private RectTransform directionIndicator;
		[SF] private RectTransform inputRect;
		[SF] private RectTransform stickHolder;
		[SF] private Image arrowImage;

		private Vector2 _startPosition;
		private Vector2 _pointerDownPosition;
		private Vector2 _direction;
		
		private IJoystickInputService _joystickInputService;

		[Inject]
		private void Construct(IJoystickInputService joystickInputService)
		{
			_joystickInputService = joystickInputService;
		}
		
		protected override void Awake()
		{
			base.Awake();
			
			_startPosition = handle.anchoredPosition;
			stickHolder.gameObject.SetActive(false);
		}

		protected override void OnUpdate()
		{
			_joystickInputService.SetJoystickDirection(_direction);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			stickHolder.gameObject.SetActive(true);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(inputRect, eventData.position, eventData.pressEventCamera, out _pointerDownPosition);
			stickHolder.anchoredPosition = _pointerDownPosition;
		}

		public void OnDrag(PointerEventData eventData)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(inputRect, eventData.position, eventData.pressEventCamera, out Vector2 position);

			Vector2 delta = position - _pointerDownPosition;

			Vector2 clampedDelta = Vector2.ClampMagnitude(delta, movementRange);

			handle.anchoredPosition = _startPosition + clampedDelta;

			_direction = new Vector2(clampedDelta.x / movementRange, clampedDelta.y / movementRange);

			float angle = Mathf.Atan2(_direction.x, _direction.y) * Mathf.Rad2Deg;

			Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, -angle));
			directionIndicator.rotation = rotation;
			arrowImage.color = new Color(1, 1, 1, Mathf.Clamp(Mathf.Abs(_direction.x) + Mathf.Abs(_direction.y), 0, 1));
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			stickHolder.gameObject.SetActive(false);
			handle.anchoredPosition = _startPosition;
			_direction = Vector2.zero;
		}
	}
}