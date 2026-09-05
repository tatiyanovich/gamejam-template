using System.Collections.Generic;
using Code.Gameplay.Exam;
using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Duck.Systems
{
	public class TickDuckStateSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _movingDucks;

		private readonly List<GameEntity> _buffer = new(1);

		public TickDuckStateSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_movingDucks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckStateTimeLeft));
		}

		public void Execute()
		{
			if (_runningExams.count == 0)
				return;

			foreach (GameEntity duck in _movingDucks.GetEntities(_buffer))
			{
				float timeLeft = duck.DuckStateTimeLeft - _timeService.DeltaTime;

				duck.ReplaceDuckStateTimeLeft(timeLeft > 0 ? timeLeft : 0f);
			}
		}
	}
}
