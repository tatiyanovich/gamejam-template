using TMPro;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class FlashRowView : MonoBehaviour
	{
		[SF] private TMP_Text label;

		private float _secondsLeft;

		public string Line => label.text;
		public Color Tint => label.color;
		public float SecondsLeft => _secondsLeft;

		public void Show(string line, Color tint, float seconds)
		{
			label.text = line;
			label.color = tint;
			_secondsLeft = seconds;
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			_secondsLeft = 0f;
			label.text = string.Empty;
			gameObject.SetActive(false);
		}

		private void Update()
		{
			if (_secondsLeft <= 0f)
				return;

			_secondsLeft -= Time.deltaTime;
			if (_secondsLeft <= 0f)
				Hide();
		}
	}
}
