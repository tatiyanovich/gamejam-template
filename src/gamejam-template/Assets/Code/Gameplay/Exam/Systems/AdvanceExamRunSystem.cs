using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class AdvanceExamRunSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _copiedQuestions;
		private readonly IGroup<GameEntity> _runs;

		private readonly List<GameEntity> _buffer = new(1);

		public AdvanceExamRunSystem(GameContext game)
		{
			_copiedQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.AnswerCopied));

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex,
					GameMatcher.AnswersCopied)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runs.GetEntities(_buffer))
			{
				foreach (GameEntity question in _copiedQuestions)
				{
					if (question.QuestionIndex != run.CurrentQuestionIndex)
						continue;

					run.ReplaceAnswersCopied(run.AnswersCopied + 1);
					run.ReplaceCurrentQuestionIndex(run.CurrentQuestionIndex + 1);
				}
			}
		}
	}
}
