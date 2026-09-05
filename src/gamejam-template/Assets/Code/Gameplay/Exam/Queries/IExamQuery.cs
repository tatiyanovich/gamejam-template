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
		event Action<int> OnWrongInput;
		event Action<ExamOutcome> OnExamFinished;
		event Action<TutorialHint> OnTutorialHintChanged;

		int GetAnswersCopied();
		int GetTotalQuestions();
		float GetElapsedSeconds();
		int GetMeowCount();
		int GetCurrentQuestionIndex();
		QuestionDefinition GetCurrentQuestion();
		int GetAnswerProgress();
		int GetAnswerLength();
		bool IsAnswerReadable();
		bool IsAnswerCopied();
		bool IsFinished();
		ExamOutcome GetOutcome();
		TutorialHint GetTutorialHint();
	}
}
