using System;
using Code.Gameplay.Exam.Data;

namespace Code.Gameplay.Exam.Queries
{
	public interface IExamQuery
	{
		event Action<int> OnAnswersCopiedChanged;
		event Action<float> OnElapsedSecondsChanged;
		event Action<int> OnCurrentQuestionChanged;
		event Action<int, int, int> OnAnswerProgressChanged;
		event Action<int, bool> OnAnswerReadableChanged;
		event Action<int> OnAnswerCopied;
		event Action<ExamOutcome> OnExamFinished;

		int GetAnswersCopied();
		int GetTotalQuestions();
		float GetElapsedSeconds();
		int GetCurrentQuestionIndex();
		QuestionDefinition GetCurrentQuestion();
		int GetAnswerProgress();
		int GetAnswerLength();
		bool IsAnswerReadable();
		bool IsAnswerCopied();
		bool IsFinished();
		ExamOutcome GetOutcome();
	}
}
