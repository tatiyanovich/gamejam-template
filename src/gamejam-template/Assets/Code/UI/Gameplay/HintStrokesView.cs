using System.Collections.Generic;
using Code.Gameplay.Exam;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class HintStrokesView : MonoBehaviour
	{
		[SF] private Image[] slots;
		[SF] private Sprite[] glyphs;
		[SF] private float centerX;
		[SF] private float advance;

		public void Show(IReadOnlyList<StrokeDirection> strokes)
		{
			int count = strokes == null ? 0 : Mathf.Min(strokes.Count, slots.Length);
			float origin = centerX - (count - 1) * advance * 0.5f;

			for (int index = 0; index < slots.Length; index++)
			{
				slots[index].gameObject.SetActive(index < count);

				if (index >= count)
					continue;

				slots[index].sprite = glyphs[(int)strokes[index] - 1];
				RectTransform slot = slots[index].rectTransform;
				slot.anchoredPosition = new Vector2(
					origin + index * advance - slot.sizeDelta.x * 0.5f,
					slot.anchoredPosition.y);
			}
		}
	}
}
