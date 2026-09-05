using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class AdvanceExamRunOnAnswerCopiedSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _answerCopiedEvents;
		private readonly IGroup<GameEntity> _runs;

		private readonly List<GameEntity> _buffer = new(1);

		public AdvanceExamRunOnAnswerCopiedSystem(GameContext game)
		{
			_answerCopiedEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.AnswerCopiedEvent));

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
			int copiedCount = _answerCopiedEvents.count;

			if (copiedCount == 0)
				return;

			foreach (GameEntity run in _runs.GetEntities(_buffer))
			{
				run.ReplaceAnswersCopied(run.AnswersCopied + copiedCount);
				run.ReplaceCurrentQuestionIndex(run.CurrentQuestionIndex + copiedCount);
			}
		}
	}
}
