using System;
using System.Collections.Generic;
using Code.Gameplay.Neighbours;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Exam.Data
{
	[Serializable]
	public class QuestionDefinition
	{
		[SF] private string text;
		[SF] private QuestionType type;
		[SF] private NeighbourSide neighbour;
		[SF] private StrokeDirection[] strokes;
		[SF] private string[] options;
		[SF] private int correctOptionIndex;
		[SF] private string word;

		public string Text => text;
		public QuestionType Type => type;
		public NeighbourSide Neighbour => neighbour;
		public IReadOnlyList<StrokeDirection> Strokes => strokes;
		public IReadOnlyList<string> Options => options;
		public int CorrectOptionIndex => correctOptionIndex;
		public string Word => word;
	}
}
