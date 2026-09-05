using System.Collections.Generic;
using Code.Gameplay.Exam;
using Code.Gameplay.Meow;
using Code.Gameplay.Suspicion.Configs;
using Code.Gameplay.Suspicion.Services;
using Code.Gameplay.Teacher;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Suspicion.Systems
{
	public class AddSuspicionOnMeowWhileWatchedSystem : IExecuteSystem
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;

		private readonly IGroup<GameEntity> _meowEvents;
		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _watchingTeachers;

		private readonly List<GameEntity> _buffer = new(1);

		public AddSuspicionOnMeowWhileWatchedSystem(GameContext game, ISuspicionConfigsService suspicionConfigsService)
		{
			_suspicionConfigsService = suspicionConfigsService;

			_meowEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.MeowEvent));

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
		}

		public void Execute()
		{
			if (_meowEvents.count == 0 || _watchingTeachers.count == 0)
				return;

			SuspicionConfig config = _suspicionConfigsService.SuspicionConfig;

			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				run.ChangeSuspicion(config.MeowWhileWatchedPenalty * _meowEvents.count, config.MaximumLevel);
			}
		}
	}
}
