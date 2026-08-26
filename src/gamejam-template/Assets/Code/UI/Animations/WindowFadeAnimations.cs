using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Animations
{
	[RequireComponent(typeof(CanvasGroup))]
	public class WindowFadeAnimations : MonoBehaviour, IUiAnimations
	{
		[SF] private CanvasGroup fadeCanvasGroup;
		[Space] 
		[SF] private float fadeDuration = 0.1f;

		private Tween _fadeTween;

		public void Initialize()
		{
			fadeCanvasGroup.alpha = 0;
		}

		public async UniTask PlayOpenAnimation(Action onComplete = null, CancellationToken cancellationToken = default)
		{
			_fadeTween.Kill();
			_fadeTween = CreateFadeInAnimation();
			await _fadeTween.SetUpdate(true).ToUniTask(cancellationToken: cancellationToken);
			onComplete?.Invoke();
		}

		public async UniTask PlayCloseAnimation(Action onComplete = null, CancellationToken cancellationToken = default)
		{
			_fadeTween.Kill();
			_fadeTween = CreateFadeOutAnimation();
			await _fadeTween.SetUpdate(true).ToUniTask(cancellationToken: cancellationToken);

			onComplete?.Invoke();
		}

		public void PlayIdleAnimation()
		{
			//Nothing to idle.
		}

		private Tween CreateFadeInAnimation()
		{
			float currentAlpha = fadeCanvasGroup.alpha;
			float targetAlpha = 1f;
			float duration = fadeDuration * Mathf.Abs(targetAlpha - currentAlpha);

			return DOTween.Sequence().Append(fadeCanvasGroup.DOFade(targetAlpha, duration));
		}

		private Tween CreateFadeOutAnimation()
		{
			float currentAlpha = fadeCanvasGroup.alpha;
			float targetAlpha = 0f;
			float duration = fadeDuration * Mathf.Abs(targetAlpha - currentAlpha);

			return DOTween.Sequence().Append(fadeCanvasGroup.DOFade(targetAlpha, duration));
		}
	}
}