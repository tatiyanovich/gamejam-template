using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Animations
{
	[RequireComponent(typeof(Image))]
	[DisallowMultipleComponent]
	public class AttentionPulseAnimation : MonoBehaviour
	{
		[SF, Range(0f, 1f)] private float minAlpha = 0.2f;
		[SF, Range(0f, 1f)] private float maxAlpha = 1f;

		[SF, Min(0.05f)] private float duration = 0.8f;

		private Image _image;

		private Tween _tween;

		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		private void OnEnable()
		{
			SetAlpha(minAlpha);

			_tween = _image
				.DOFade(maxAlpha, duration)
				.SetUpdate(true)
				.SetEase(Ease.InOutSine)
				.SetLoops(-1, LoopType.Yoyo);
		}

		private void OnDisable()
		{
			if (_tween != null)
			{
				_tween.Kill();
				_tween = null;
			}

			SetAlpha(minAlpha);
		}

		private void SetAlpha(float alpha)
		{
			Color color = _image.color;
			color.a = alpha;

			_image.color = color;
		}
	}
}
