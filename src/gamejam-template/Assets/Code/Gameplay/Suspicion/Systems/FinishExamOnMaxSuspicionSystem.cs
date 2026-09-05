using System.Collections.Generic;
using Code.Gameplay.Exam;
using Code.Gameplay.Suspicion.Services;
using Entitas;

namespace Code.Gameplay.Suspicion.Systems
{
	public class FinishExamOnMaxSuspicionSystem : IExecuteSystem
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;

		private readonly IGroup<GameEntity> _runningExams;

		private readonly List<GameEntity> _buffer = new(1);

		public FinishExamOnMaxSuspicionSystem(GameContext game, ISuspicionConfigsService suspicionConfigsService)
		{
			_suspicionConfigsService = suspicionConfigsService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				if (run.SuspicionLevel < _suspicionConfigsService.SuspicionConfig.MaximumLevel)
					continue;

				run.isExamFinished = true;
				run.ReplaceExamOutcome(ExamOutcome.Caught);
			}
		}
	}
}
