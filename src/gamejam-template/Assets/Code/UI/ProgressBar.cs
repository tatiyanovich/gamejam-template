using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI
{
	[ExecuteAlways]
	public class ProgressBar : MonoBehaviour
    {
		[SF] private float fillDuration;
		[SF] private AnimationCurve ease;
		[SF] private float delay = 0.3f;
		[SF] private bool reverse;
		[SF] private Image fillImage;
		[SF] private Image[] stripesImages;
		[SF] private RectTransform fill;

		private bool _initialValueChange;
		private float _value;

		public event Action<float, bool> OnProgressChange;
		public event Action OnProgressFilled;

		public float Value
		{
			get => _value;

			set
			{
				if (Math.Abs(_value - value) > 0.001f || _initialValueChange == false)
				{
					_initialValueChange = true;
					_value = Mathf.Clamp01(value);

					if (Application.isPlaying == false)
					{
						if (reverse)
						{
							fill.anchorMin = new Vector2(1 - _value, fill.anchorMin.y);
						}
						else
						{
							fill.anchorMax = new Vector2(_value, fill.anchorMax.y);
						}
					}
					else
						Evaluate(_value);

					OnProgressChange?.Invoke(_value, false);
				}
			}
		}

		public float DirectValue
		{
			get => _value;

			set
			{
				if (Math.Abs(_value - value) > 0.001f || _initialValueChange == false)
				{
					_initialValueChange = true;

					_value = Mathf.Clamp01(value);

					fill.DOKill();

					if (reverse)
					{
						fill.anchorMin = new Vector2(1 - _value, fill.anchorMin.y);
					}
					else
					{
						fill.anchorMax = new Vector2(_value, fill.anchorMax.y);
					}

					OnProgressChange?.Invoke(_value, true);
				}
			}
		}
		public void SetBarColor(Color32 color)
		{
			fillImage.color = color;

			foreach (Image stripesImage in stripesImages)
			{
				stripesImage.color = color;
			}
		}

		private void Evaluate(float normalizedValue)
		{
			fill.DOKill();

			if (reverse)
			{
				fill
					.DOAnchorMin(new Vector2(1 - normalizedValue, fill.anchorMin.y), fillDuration)
					.SetEase(ease)
					.SetDelay(delay)
					.SetUpdate(true);
			}
			else
			{
				fill.DOAnchorMax(new Vector2(normalizedValue, fill.anchorMax.y), fillDuration)
					.SetEase(ease)
					.SetDelay(delay)
					.SetUpdate(true)
					.OnComplete(() =>
					{
						if (IsFilled())
							OnProgressFilled?.Invoke();
					});
			}
		}

		public void SetFillDuration(float duration)
		{
			fillDuration = duration;
		}

		public bool IsFilled() => 
			fill.anchorMax.x >= 0.99f;

		private void OnDestroy() => 
			fill?.DOKill();
    }
}
