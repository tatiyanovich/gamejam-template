using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class DangerVignetteView : MonoBehaviour
	{
		[SF] private Image image;

		private bool _isVisible;
		private bool _isPulsing;
		private float _pulseSeconds;

		private static readonly Color Warn = new Color32(255, 154, 61, 255);
		private static readonly Color Danger = new Color32(232, 76, 76, 255);

		private const float CalmAlpha = 0.35f;
		private const float PulseMinimumAlpha = 0.35f;
		private const float PulseMaximumAlpha = 0.6f;
		private const float PulseSeconds = 0.4f;

		public void Show(bool isPulsing)
		{
			if (_isVisible == false || _isPulsing != isPulsing)
				_pulseSeconds = 0f;

			_isVisible = true;
			_isPulsing = isPulsing;
			Refresh();
		}

		public void Hide()
		{
			_isVisible = false;
			_isPulsing = false;
			_pulseSeconds = 0f;
			Refresh();
		}

		private void Update()
		{
			if (_isVisible == false || _isPulsing == false)
				return;

			_pulseSeconds += Time.deltaTime;
			Refresh();
		}

		private void Refresh()
		{
			Color color = _isPulsing ? Danger : Warn;
			color.a = GetAlpha();
			image.color = color;
		}

		private float GetAlpha()
		{
			if (_isVisible == false)
				return 0f;

			if (_isPulsing == false)
				return CalmAlpha;

			float phase = 0.5f - 0.5f * Mathf.Cos(_pulseSeconds * Mathf.PI * 2f / PulseSeconds);
			return Mathf.Lerp(PulseMinimumAlpha, PulseMaximumAlpha, phase);
		}
	}
}
