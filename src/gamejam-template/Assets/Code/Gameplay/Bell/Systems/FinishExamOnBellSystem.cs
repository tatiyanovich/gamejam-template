using System.Collections.Generic;
using Code.Gameplay.Bell.Services;
using Code.Gameplay.Exam;
using Entitas;

namespace Code.Gameplay.Bell.Systems
{
	public class FinishExamOnBellSystem : IExecuteSystem
	{
		private readonly IBellConfigsService _bellConfigsService;

		private readonly IGroup<GameEntity> _runningExams;

		private readonly List<GameEntity> _buffer = new(1);

		public FinishExamOnBellSystem(GameContext game, IBellConfigsService bellConfigsService)
		{
			_bellConfigsService = bellConfigsService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamElapsedSeconds)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				if (run.ExamElapsedSeconds < _bellConfigsService.BellConfig.ExamSeconds)
					continue;

				run.isExamFinished = true;
				run.ReplaceExamOutcome(ExamOutcome.BellRang);
			}
		}
	}
}
