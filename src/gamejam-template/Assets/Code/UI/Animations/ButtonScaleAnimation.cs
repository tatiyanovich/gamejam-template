using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Framework.UI.UiManagement.Elements.Buttons;
using UnityEngine;
using UnityEngine.EventSystems;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Animations
{
	[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
	[DisallowMultipleComponent]
	public class ButtonScaleAnimation : MonoBehaviour, IButtonAnimation, IPointerEnterHandler, IPointerExitHandler
	{
		[SF] protected float scaleDownDuration = 0.1f;
		[SF] protected float scaleUpDuration = 0.1f;

		[SF] protected float scaleFactor = 0.9f;

		[SF, Min(1f)] private float hoverScaleFactor = 1.06f;
		[SF, Min(0f)] private float hoverDuration = 0.12f;

		[SF] private Vector3 originalScale = Vector3.one;
		[SF] private float blockedAlpha = 0.6f;

		private RectTransform _rectTransform;
		private CanvasGroup _canvasGroup;
		private Button _button;

		private TweenerCore<Vector3, Vector3, VectorOptions> _pressAnimationTween;
		private TweenerCore<Vector3, Vector3, VectorOptions> _releaseAnimationTween;

		private bool _isPointerOver;

		private const int MousePointerId = -1;

		private void Awake()
		{
			CacheComponents();
		}

		public virtual void Initialize(Button button)
		{
			CacheComponents();
			_button = button;
		}

		public void PlayPressAnimation(Action onPress)
		{
			if (_pressAnimationTween != null)
				return;

			_rectTransform.DOKill(true);

			_pressAnimationTween = _rectTransform
				.DOScale(originalScale * scaleFactor, scaleDownDuration)
				.SetUpdate(true)
				.OnComplete(() =>
				{
					_pressAnimationTween = null;
					_releaseAnimationTween = null;
					onPress?.Invoke();
				});
		}

		public void PlayReleaseAnimation(Action onRelease, Action onClicked)
		{
			onClicked?.Invoke();

			if (_releaseAnimationTween != null)
				return;

			if (_pressAnimationTween != null)
			{
				_pressAnimationTween.Kill();
				_pressAnimationTween = null;
			}

			_rectTransform.DOKill(true);
			_releaseAnimationTween = _rectTransform
				.DOScale(RestingScale(), scaleUpDuration)
				.SetUpdate(true)
				.OnComplete(() =>
				{
					_pressAnimationTween = null;
					_releaseAnimationTween = null;
					onRelease?.Invoke();
				});
		}

		public virtual void SetInteractable(bool state)
		{
			_canvasGroup.interactable = state;
			_canvasGroup.blocksRaycasts = state;
			_canvasGroup.alpha = state ? 1f : blockedAlpha;

			if (state)
				return;

			_isPointerOver = false;
			_rectTransform.DOKill();
			_rectTransform.localScale = originalScale;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (CanHover(eventData) == false)
				return;

			_isPointerOver = true;
			PlayHoverAnimation();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (CanHover(eventData) == false)
				return;

			_isPointerOver = false;

			if (_pressAnimationTween != null || _releaseAnimationTween != null)
				return;

			PlayHoverAnimation();
		}

		private void OnDisable()
		{
			_isPointerOver = false;
			_pressAnimationTween = null;
			_releaseAnimationTween = null;

			if (_rectTransform == null)
				return;

			_rectTransform.DOKill();
			_rectTransform.localScale = originalScale;
		}

		private void CacheComponents()
		{
			if (_rectTransform != null)
				return;

			_rectTransform = GetComponent<RectTransform>();
			_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void PlayHoverAnimation()
		{
			_rectTransform.DOKill();
			_rectTransform
				.DOScale(RestingScale(), hoverDuration)
				.SetUpdate(true)
				.SetEase(Ease.OutQuad);
		}

		private Vector3 RestingScale()
		{
			return _isPointerOver ? originalScale * hoverScaleFactor : originalScale;
		}

		private bool CanHover(PointerEventData eventData)
		{
			if (eventData.pointerId != MousePointerId)
				return false;

			return _button != null && _button.IsInteractable;
		}
	}
}
