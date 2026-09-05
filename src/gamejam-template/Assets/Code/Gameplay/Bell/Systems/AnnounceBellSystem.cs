using System.Collections.Generic;
using Code.Gameplay.Bell.Configs;
using Code.Gameplay.Bell.Services;
using Code.Gameplay.Exam;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Bell.Systems
{
	public class AnnounceBellSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IBellConfigsService _bellConfigsService;

		private readonly IGroup<GameEntity> _runningExams;

		private readonly List<GameEntity> _buffer = new(1);

		public AnnounceBellSystem(
			GameContext game,
			IEntityFactory entityFactory,
			IBellConfigsService bellConfigsService)
		{
			_entityFactory = entityFactory;
			_bellConfigsService = bellConfigsService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamElapsedSeconds)
				.NoneOf(
					GameMatcher.ExamFinished,
					GameMatcher.BellAnnounced));
		}

		public void Execute()
		{
			BellConfig config = _bellConfigsService.BellConfig;

			foreach (GameEntity run in _runningExams.GetEntities(_buffer))
			{
				if (run.ExamElapsedSeconds < config.ExamSeconds - config.AnnouncementSecondsLeft)
					continue;

				run.isBellAnnounced = true;

				_entityFactory.Event()
					.With(x => x.isBellAnnouncementEvent = true);
			}
		}
	}
}
