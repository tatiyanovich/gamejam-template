using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.UI.UiManagement.Elements.Buttons
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler,
		IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		[SerializeField, Tooltip("Optional text component for the button. If not set, searches in children.")]
		private TMP_Text text;

		[SerializeField, Tooltip("Distance in pixels to consider a drag instead of a click.")]
		private float dragThreshold = 100f;

		private Image _image;
		private ScrollRect _scrollRectParent;
		private IButtonAnimation _animation;

		private bool _isPressed;
		private bool _isPointerOver;
		private Vector2 _clickStartPosition;
		private bool _isDragged;

		public event Action OnPressed;
		public event Action OnReleased;
		public event Action OnClicked;

		public Image Image => _image;
		public string Text => text != null ? text.text : string.Empty;
		public bool IsInteractable { get; private set; } = true;

		private void Awake()
		{
			Initialize();
		}

		public void TriggerClick()
		{
			if (IsInteractable == false)
				return;

			OnClicked?.Invoke();
		}

		public void ClearEvents()
		{
			OnPressed = null;
			OnReleased = null;
			OnClicked = null;
		}

		public void SetInteractable(bool interactable)
		{
			IsInteractable = interactable;
			_isPointerOver = interactable && _isPointerOver;
			_animation?.SetInteractable(interactable);
			ResetState();
		}

		public void SetText(string newText)
		{
			if (text != null)
				text.text = newText;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			_isPressed = true;
			_isPointerOver = true;
			_isDragged = false;
			_clickStartPosition = eventData.position;

			EventSystem.current.SetSelectedGameObject(gameObject);

			if (_scrollRectParent != null)
			{
				_scrollRectParent.StopMovement();
				_scrollRectParent.OnBeginDrag(eventData);
			}

			_animation?.PlayPressAnimation(OnPressed);
			if (_animation == null)
				OnPressed?.Invoke();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			if (_isDragged == false && _isPointerOver)
			{
				_animation?.PlayReleaseAnimation(OnReleased, OnClicked);
				if (_animation == null)
				{
					OnReleased?.Invoke();
					OnClicked?.Invoke();
				}
			}
			else
			{
				_animation?.PlayReleaseAnimation(null, null);
			}

			ResetState();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			_isPointerOver = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			_isPointerOver = false;

			if (_isPressed)
				_animation?.PlayReleaseAnimation(OnReleased, null);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			if (_scrollRectParent != null)
				_scrollRectParent.OnBeginDrag(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			float dragDistance = Vector2.Distance(_clickStartPosition, eventData.position);
			_isDragged = dragDistance >= dragThreshold;

			if (_isDragged)
				OnPointerExit(eventData);

			if (_scrollRectParent != null)
				_scrollRectParent.OnDrag(eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (IsInteractable == false)
				return;

			if (_scrollRectParent != null)
				_scrollRectParent.OnEndDrag(eventData);
		}

		private void Initialize()
		{
			_image = GetComponent<Image>();

			if (text == null)
				text = GetComponentInChildren<TMP_Text>();

			_animation = GetComponent<IButtonAnimation>();
			_animation?.Initialize(this);

			CacheScrollRectParent();
		}

		private void CacheScrollRectParent()
		{
			_scrollRectParent = GetComponentInParent<ScrollRect>(true);
		}

		private void OnTransformParentChanged()
		{
			CacheScrollRectParent();
		}

		private void ResetState()
		{
			_isPressed = false;
			_isDragged = false;
		}
	}
}
