using System.Collections.Generic;
using Code.Gameplay.Exam;
using Code.Gameplay.Suspicion.Configs;
using Code.Gameplay.Suspicion.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;

namespace Code.Gameplay.Suspicion.Systems
{
	public class AddSuspicionOnWrongInputSystem : IExecuteSystem
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;

		private readonly IGroup<GameEntity> _wrongInputEvents;
		private readonly IGroup<GameEntity> _runningExams;

		private readonly List<GameEntity> _buffer = new(1);

		public AddSuspicionOnWrongInputSystem(GameContext game, ISuspicionConfigsService suspicionConfigsService)
		{
			_suspicionConfigsService = suspicionConfigsService;

			_wrongInputEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.WrongInputEvent));

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			if (_wrongInputEvents.count == 0)
				return;

			SuspicionConfig config = _suspicionConfigsService.SuspicionConfig;

			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				run.ChangeSuspicion(config.WrongInputPenalty * _wrongInputEvents.count, config.MaximumLevel);
			}
		}
	}
}
