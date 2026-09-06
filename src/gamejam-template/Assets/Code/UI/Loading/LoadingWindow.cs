using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Loading
{
	public class LoadingWindow : WindowBase
	{
		[SF] private RectTransform layout;
		[SF] private RectTransform logo;
		[SF] private RectTransform duck;
		[SF] private Image progressFill;
		[SF] private TMP_Text status;

		private Tween _duckBob;
		private Tween _logoPulse;
		private float _duckBaseHeight;

		private const float FillDuration = 0.35f;
		private const float DuckBobHeight = 14f;
		private const float DuckBobDuration = 0.6f;
		private const float LogoPulseScale = 1.03f;
		private const float LogoPulseDuration = 1.4f;

		private static readonly string[] StatusLines =
		{
			"Sharpening pencils…",
			"Waking up Fluffy…",
			"Hiding the duck…"
		};

		public override void Dispose()
		{
			StopIdleAnimations();
			progressFill.DOKill();
		}

		private void OnRectTransformDimensionsChange()
		{
			if (layout == null)
				return;

			Rect bounds = ((RectTransform)transform).rect;
			layout.localScale = Vector3.one * Mathf.Min(bounds.width / 1920f, bounds.height / 1080f);
		}

		protected override UniTask OnInitialize(CancellationToken cancellationToken = new CancellationToken())
		{
			_duckBaseHeight = duck.anchoredPosition.y;
			return base.OnInitialize(cancellationToken);
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = new CancellationToken())
		{
			OnRectTransformDimensionsChange();
			progressFill.DOKill();
			progressFill.fillAmount = 0f;
			status.text = StatusLines[0];
			PlayIdleAnimations();
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = new CancellationToken())
		{
			StopIdleAnimations();
			return base.OnClose(cancellationToken);
		}

		public void SetProgress(float progress)
		{
			float value = Mathf.Clamp01(progress);

			progressFill.DOKill();
			progressFill
				.DOFillAmount(value, FillDuration)
				.SetEase(Ease.OutQuad)
				.SetUpdate(true);

			status.text = StatusLines[Mathf.Clamp(Mathf.FloorToInt(value * StatusLines.Length), 0, StatusLines.Length - 1)];
		}

		private void PlayIdleAnimations()
		{
			StopIdleAnimations();

			_duckBob = duck
				.DOAnchorPosY(_duckBaseHeight + DuckBobHeight, DuckBobDuration)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(true);

			_logoPulse = logo
				.DOScale(LogoPulseScale, LogoPulseDuration)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo)
				.SetUpdate(true);
		}

		private void StopIdleAnimations()
		{
			_duckBob?.Kill();
			_duckBob = null;
			_logoPulse?.Kill();
			_logoPulse = null;

			if (duck != null)
				duck.anchoredPosition = new Vector2(duck.anchoredPosition.x, _duckBaseHeight);

			if (logo != null)
				logo.localScale = Vector3.one;
		}
	}
}
