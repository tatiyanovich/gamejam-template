using System.Collections.Generic;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Exam.Behaviours
{
	public class PaperGlyphRow : MonoBehaviour
	{
		[SF] private SpriteRenderer[] slots;
		[SF] private Sprite[] normalGlyphs;
		[SF] private Sprite[] doneGlyphs;
		[SF] private Sprite[] wrongGlyphs;
		[SF] private float advance;
		[SF] private bool centered;

		public void Configure(PaperGlyphRowDto dto)
		{
			slots = dto.Slots;
			normalGlyphs = dto.NormalGlyphs;
			doneGlyphs = dto.DoneGlyphs;
			wrongGlyphs = dto.WrongGlyphs;
			advance = dto.Advance;
			centered = dto.Centered;
		}

		public void ShowStrokes(IReadOnlyList<StrokeDirection> strokes, int progress, int wrongIndex)
		{
			int count = Mathf.Min(strokes.Count, slots.Length);
			float origin = centered ? -(count - 1) * advance * 0.5f : 0f;

			for (int index = 0; index < slots.Length; index++)
			{
				slots[index].gameObject.SetActive(index < count);

				if (index >= count)
					continue;

				Sprite[] glyphs = index == wrongIndex ? wrongGlyphs : index < progress ? doneGlyphs : normalGlyphs;
				slots[index].sprite = glyphs[(int)strokes[index] - 1];
				slots[index].transform.localPosition = new Vector3(origin + index * advance, 0f, 0f);
			}
		}

		public void ShowTypedStrokes(IReadOnlyList<StrokeDirection> strokes, int progress)
		{
			int count = Mathf.Min(Mathf.Min(strokes.Count, progress), slots.Length);
			float origin = centered ? -(count - 1) * advance * 0.5f : 0f;

			for (int index = 0; index < slots.Length; index++)
			{
				slots[index].gameObject.SetActive(index < count);

				if (index >= count)
					continue;

				slots[index].sprite = doneGlyphs[(int)strokes[index] - 1];
				slots[index].transform.localPosition = new Vector3(origin + index * advance, 0f, 0f);
			}
		}

		public void Clear()
		{
			foreach (SpriteRenderer slot in slots)
				slot.gameObject.SetActive(false);
		}
	}
}
