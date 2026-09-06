using TMPro;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Exam.Behaviours
{
	public class PaperLetterRow : MonoBehaviour
	{
		[SF] private Transform[] slots;
		[SF] private SpriteRenderer[] backgrounds;
		[SF] private TMP_Text[] letters;
		[SF] private Sprite normalGlyph;
		[SF] private Sprite doneGlyph;
		[SF] private Sprite wrongGlyph;
		[SF] private Color normalColor;
		[SF] private Color completedColor;
		[SF] private float advance;

		public void Configure(PaperLetterRowDto dto)
		{
			slots = dto.Slots;
			backgrounds = dto.Backgrounds;
			letters = dto.Letters;
			normalGlyph = dto.NormalGlyph;
			doneGlyph = dto.DoneGlyph;
			wrongGlyph = dto.WrongGlyph;
			normalColor = dto.NormalColor;
			completedColor = dto.CompletedColor;
			advance = dto.Advance;
		}

		public void ShowWord(string word, int progress, int wrongIndex)
		{
			int count = Mathf.Min(word.Length, slots.Length);
			float origin = -(count - 1) * advance * 0.5f;

			for (int index = 0; index < slots.Length; index++)
			{
				slots[index].gameObject.SetActive(index < count);

				if (index >= count)
					continue;

				bool wrong = index == wrongIndex;
				bool done = index < progress;
				backgrounds[index].sprite = wrong ? wrongGlyph : done ? doneGlyph : normalGlyph;
				letters[index].text = word[index].ToString();
				letters[index].color = wrong || done ? completedColor : normalColor;
				slots[index].localPosition = new Vector3(origin + index * advance, 0f, 0f);
			}
		}

		public void Clear()
		{
			foreach (Transform slot in slots)
				slot.gameObject.SetActive(false);
		}
	}
}
