using TMPro;
using UnityEngine;

namespace Code.Gameplay.Exam.Behaviours
{
	public struct PaperLetterRowDto
	{
		public Transform[] Slots;
		public SpriteRenderer[] Backgrounds;
		public TMP_Text[] Letters;
		public Sprite NormalGlyph;
		public Sprite DoneGlyph;
		public Sprite WrongGlyph;
		public Color NormalColor;
		public Color CompletedColor;
		public float Advance;
	}
}
