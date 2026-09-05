using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public sealed class UpdateTutorialHintSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runningRuns;
		private readonly IGroup<GameEntity> _finishedRuns;
		private readonly IGroup<GameEntity> _readableAnswers;

		private readonly List<GameEntity> _buffer = new(1);

		private const int DodgeHintQuestionIndex = 3;
		private const int DuckHintQuestionIndex = 5;

		public UpdateTutorialHintSystem(GameContext game)
		{
			_runningRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex,
					GameMatcher.AnswersCopied,
					GameMatcher.TutorialHint)
				.NoneOf(
					GameMatcher.ExamFinished));

			_finishedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.TutorialHint,
					GameMatcher.ExamFinished));

			_readableAnswers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerReadable)
				.NoneOf(
					GameMatcher.AnswerCopied));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runningRuns.GetEntities(_buffer))
				SetHint(run, GetHint(run));

			foreach (GameEntity run in _finishedRuns.GetEntities(_buffer))
				SetHint(run, TutorialHint.None);
		}

		private void SetHint(GameEntity run, TutorialHint hint)
		{
			if (run.TutorialHint != hint)
				run.ReplaceTutorialHint(hint);
		}

		private TutorialHint GetHint(GameEntity run)
		{
			if (run.isTutorialMeowed == false)
				return TutorialHint.Meow;

			if (run.isTutorialLeaned == false)
				return TutorialHint.Lean;

			if (run.AnswersCopied == 0 && IsAnswerReadable(run.CurrentQuestionIndex))
				return TutorialHint.Copy;

			if (run.CurrentQuestionIndex >= DuckHintQuestionIndex && run.isTutorialDuckThrown == false)
				return TutorialHint.Duck;

			if (run.CurrentQuestionIndex >= DodgeHintQuestionIndex && run.isTutorialDodgedTeacher == false)
				return TutorialHint.Dodge;

			return TutorialHint.None;
		}

		private bool IsAnswerReadable(int questionIndex)
		{
			foreach (GameEntity answer in _readableAnswers)
			{
				if (answer.QuestionIndex == questionIndex)
					return true;
			}

			return false;
		}
	}
}
