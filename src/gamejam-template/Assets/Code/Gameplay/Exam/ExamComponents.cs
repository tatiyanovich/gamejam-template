using System.Collections.Generic;
using Code.Gameplay.Neighbours;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace Code.Gameplay.Exam
{
	[Game] public class ExamRun : IComponent { }
	[Game, Watched] public class CurrentQuestionIndex : IComponent { public int Value; }
	[Game, Watched] public class AnswersCopied : IComponent { public int Value; }
	[Game, Watched] public class ExamElapsedSeconds : IComponent { public float Value; }
	[Game, Watched] public class ExamFinished : IComponent { }
	[Game, Watched] public class ExamOutcomeComponent : IComponent { public ExamOutcome Value; }

	[Game] public class Question : IComponent { }
	[Game] public class QuestionIndex : IComponent { public int Value; }
	[Game] public class QuestionText : IComponent { public string Value; }
	[Game] public class QuestionTypeComponent : IComponent { public QuestionType Value; }
	[Game] public class AnswerNeighbourSide : IComponent { public NeighbourSide Value; }
	[Game] public class AnswerStrokes : IComponent { public IReadOnlyList<StrokeDirection> Value; }
	[Game] public class AnswerOptions : IComponent { public IReadOnlyList<string> Value; }
	[Game] public class CorrectOptionIndex : IComponent { public int Value; }
	[Game] public class AnswerWord : IComponent { public string Value; }
	[Game] public class AnswerLength : IComponent { public int Value; }
	[Game, Watched] public class AnswerProgress : IComponent { public int Value; }
	[Game, Watched] public class AnswerCopied : IComponent { }

	[Game]
	public class AnswerCopiedEvent : IComponent
	{
		public int QuestionIndex;
	}
}
