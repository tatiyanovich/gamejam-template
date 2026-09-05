using System.Collections.Generic;
using Code.Gameplay.Exam.Services;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class FinishExamOnLastAnswerSystem : IExecuteSystem
	{
		private readonly IExamConfigsService _examConfigsService;

		private readonly IGroup<GameEntity> _runs;

		private readonly List<GameEntity> _buffer = new(1);

		public FinishExamOnLastAnswerSystem(GameContext game, IExamConfigsService examConfigsService)
		{
			_examConfigsService = examConfigsService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.AnswersCopied)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runs.GetEntities(_buffer))
			{
				if (run.AnswersCopied < _examConfigsService.ExamConfig.Questions.Count)
					continue;

				run.isExamFinished = true;
				run.ReplaceExamOutcome(ExamOutcome.Passed);
			}
		}
	}
}
