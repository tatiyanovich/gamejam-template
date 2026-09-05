using System;
using Code.Gameplay.Exam.Data;
using Code.Gameplay.Exam.Services;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Exam.Queries
{
	public sealed class ExamQuery : IExamQuery, IReactiveQuery
	{
		private readonly IExamConfigsService _examConfigsService;

		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _changedRuns;
		private readonly IGroup<GameEntity> _questions;
		private readonly IGroup<GameEntity> _changedQuestions;

		public event Action<int> OnAnswersCopiedChanged;
		public event Action<float> OnElapsedSecondsChanged;
		public event Action<int> OnCurrentQuestionChanged;
		public event Action<int, int, int> OnAnswerProgressChanged;
		public event Action<int, bool> OnAnswerReadableChanged;
		public event Action<int> OnAnswerCopied;
		public event Action<ExamOutcome> OnExamFinished;

		public ExamQuery(GameContext game, IExamConfigsService examConfigsService)
		{
			_examConfigsService = examConfigsService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex,
					GameMatcher.AnswersCopied,
					GameMatcher.ExamElapsedSeconds,
					GameMatcher.ExamOutcome));

			_changedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex,
					GameMatcher.AnswersCopied,
					GameMatcher.ExamElapsedSeconds,
					GameMatcher.ExamOutcome)
				.AnyOf(
					GameMatcher.CurrentQuestionIndexChanged,
					GameMatcher.AnswersCopiedChanged,
					GameMatcher.ExamElapsedSecondsChanged,
					GameMatcher.ExamFinishedChanged));

			_questions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerProgress,
					GameMatcher.AnswerLength));

			_changedQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerProgress,
					GameMatcher.AnswerLength)
				.AnyOf(
					GameMatcher.AnswerProgressChanged,
					GameMatcher.AnswerReadableChanged,
					GameMatcher.AnswerCopiedChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity run in _changedRuns)
			{
				if (run.isAnswersCopiedChanged)
					OnAnswersCopiedChanged?.Invoke(run.AnswersCopied);

				if (run.isExamElapsedSecondsChanged)
					OnElapsedSecondsChanged?.Invoke(run.ExamElapsedSeconds);

				if (run.isCurrentQuestionIndexChanged)
					OnCurrentQuestionChanged?.Invoke(run.CurrentQuestionIndex);

				if (run.isExamFinishedChanged && run.isExamFinished)
					OnExamFinished?.Invoke(run.ExamOutcome);
			}

			foreach (GameEntity question in _changedQuestions)
			{
				if (question.isAnswerProgressChanged)
				{
					OnAnswerProgressChanged?.Invoke(
						question.QuestionIndex,
						question.AnswerProgress,
						question.AnswerLength);
				}

				if (question.isAnswerReadableChanged)
					OnAnswerReadableChanged?.Invoke(question.QuestionIndex, question.isAnswerReadable);

				if (question.isAnswerCopiedChanged && question.isAnswerCopied)
					OnAnswerCopied?.Invoke(question.QuestionIndex);
			}
		}

		public int GetAnswersCopied()
		{
			foreach (GameEntity run in _runs)
				return run.AnswersCopied;

			return 0;
		}

		public int GetTotalQuestions() => _examConfigsService.ExamConfig.Questions.Count;

		public float GetElapsedSeconds()
		{
			foreach (GameEntity run in _runs)
				return run.ExamElapsedSeconds;

			return 0f;
		}

		public int GetCurrentQuestionIndex()
		{
			foreach (GameEntity run in _runs)
				return run.CurrentQuestionIndex;

			return -1;
		}

		public QuestionDefinition GetCurrentQuestion()
		{
			int questionIndex = GetCurrentQuestionIndex();

			if (questionIndex < 0 || questionIndex >= GetTotalQuestions())
				return null;

			return _examConfigsService.ExamConfig.Questions[questionIndex];
		}

		public int GetAnswerProgress()
		{
			GameEntity question = GetCurrentQuestionEntity();
			return question == null ? 0 : question.AnswerProgress;
		}

		public int GetAnswerLength()
		{
			GameEntity question = GetCurrentQuestionEntity();
			return question == null ? 0 : question.AnswerLength;
		}

		public bool IsAnswerReadable()
		{
			GameEntity question = GetCurrentQuestionEntity();
			return question != null && question.isAnswerReadable;
		}

		public bool IsAnswerCopied()
		{
			GameEntity question = GetCurrentQuestionEntity();
			return question != null && question.isAnswerCopied;
		}

		public bool IsFinished()
		{
			foreach (GameEntity run in _runs)
				return run.isExamFinished;

			return false;
		}

		public ExamOutcome GetOutcome()
		{
			foreach (GameEntity run in _runs)
				return run.ExamOutcome;

			return ExamOutcome.None;
		}

		private GameEntity GetCurrentQuestionEntity()
		{
			int questionIndex = GetCurrentQuestionIndex();

			foreach (GameEntity question in _questions)
			{
				if (question.QuestionIndex == questionIndex)
					return question;
			}

			return null;
		}
	}
}
