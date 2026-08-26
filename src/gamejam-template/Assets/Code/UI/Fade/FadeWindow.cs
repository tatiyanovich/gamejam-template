using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Fade
{
	public class FadeWindow : WindowBase
	{
		[SF] private Canvas canvas;
		[SF] private CanvasGroup canvasGroup;
		[SF] private AnimationCurve fadeCurve;

		private Tween _tween;

		public UniTask FadeIn(float duration, CancellationToken cancellationToken = default)
		{
			canvas.sortingOrder = 1000;
			return FadeTo(targetAlpha: 1f, duration, cancellationToken);
		}

		public async UniTask FadeOut(float duration, float delay = 0f, CancellationToken cancellationToken = default)
		{
			if (delay > 0f)
				await UniTask.Delay(
					TimeSpan.FromSeconds(delay),
					ignoreTimeScale: true,
					cancellationToken: cancellationToken);

			await FadeTo(targetAlpha: 0f, duration, cancellationToken);
		}

		private UniTask FadeTo(float targetAlpha, float duration, CancellationToken cancellationToken)
		{
			_tween?.Kill();

			canvasGroup.blocksRaycasts = targetAlpha > 0f;

			if (duration <= 0f)
			{
				canvasGroup.alpha = targetAlpha;
				return UniTask.CompletedTask;
			}

			_tween = canvasGroup
				.DOFade(targetAlpha, duration)
				.SetEase(fadeCurve)
				.SetUpdate(true);

			return _tween.ToUniTask(cancellationToken: cancellationToken);
		}
	}
}
