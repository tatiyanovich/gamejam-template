using System.Collections.Generic;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Suspicion.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Suspicion.Systems
{
	public class ReduceSuspicionOnDuckThrowSystem : IExecuteSystem
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _duckThrownEvents;
		private readonly IGroup<GameEntity> _runningExams;

		private readonly List<GameEntity> _buffer = new(1);

		public ReduceSuspicionOnDuckThrowSystem(
			GameContext game,
			ISuspicionConfigsService suspicionConfigsService,
			IDuckConfigsService duckConfigsService)
		{
			_suspicionConfigsService = suspicionConfigsService;
			_duckConfigsService = duckConfigsService;

			_duckThrownEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.DuckThrownEvent));

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			if (_duckThrownEvents.count == 0)
				return;

			float relief = _duckConfigsService.DuckConfig.SuspicionRelief * _duckThrownEvents.count;

			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				run.ChangeSuspicion(-relief, _suspicionConfigsService.SuspicionConfig.MaximumLevel);
			}
		}
	}
}
