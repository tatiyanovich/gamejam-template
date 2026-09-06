using UnityEngine;

namespace Code.Gameplay.Exam.Behaviours
{
	public struct PaperGlyphRowDto
	{
		public SpriteRenderer[] Slots;
		public Sprite[] NormalGlyphs;
		public Sprite[] DoneGlyphs;
		public Sprite[] WrongGlyphs;
		public float Advance;
		public bool Centered;
	}
}
