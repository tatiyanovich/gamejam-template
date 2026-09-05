using System.Collections.Generic;
using Code.Gameplay.Exam;
using Code.Gameplay.Input;
using Code.Gameplay.Suspicion.Configs;
using Code.Gameplay.Suspicion.Services;
using Code.Gameplay.Teacher;
using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Suspicion.Systems
{
	public class AccumulateSuspicionWhileWatchedSystem : IExecuteSystem
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _watchingTeachers;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public AccumulateSuspicionWhileWatchedSystem(
			GameContext game,
			InputContext input,
			ISuspicionConfigsService suspicionConfigsService,
			ITimeService timeService)
		{
			_suspicionConfigsService = suspicionConfigsService;
			_timeService = timeService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel)
				.NoneOf(
					GameMatcher.ExamFinished));

			_watchingTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherFacingClass));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			if (_leaningInputs.count == 0 || _watchingTeachers.count == 0)
				return;

			SuspicionConfig config = _suspicionConfigsService.SuspicionConfig;

			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				run.ChangeSuspicion(config.WatchedGainPerSecond * _timeService.DeltaTime, config.MaximumLevel);
			}
		}
	}
}
