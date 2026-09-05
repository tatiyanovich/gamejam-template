using System.Collections.Generic;
using Code.Gameplay.Duck.Services;
using Code.Gameplay.Exam;
using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public sealed class ThrowDuckByRequestSystem : RequestHandlerSystem<GameEntity>
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _ducks;

		private readonly List<GameEntity> _buffer = new(1);

		public ThrowDuckByRequestSystem(
			GameContext game,
			IEntityFactory entityFactory,
			IDuckConfigsService duckConfigsService)
			: base(game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Request,
					GameMatcher.ThrowDuckRequest)))
		{
			_entityFactory = entityFactory;
			_duckConfigsService = duckConfigsService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_ducks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckThrowCount));
		}

		protected override void OnExecute(IGroup<GameEntity> requests)
		{
			if (_runningExams.count == 0)
				return;

			foreach (GameEntity duck in _ducks.GetEntities(_buffer))
			{
				if (duck.DuckState != DuckState.OnDesk)
					continue;

				ThrowDuck(duck);
			}
		}

		private void ThrowDuck(GameEntity duck)
		{
			duck.ReplaceDuckThrowCount(duck.DuckThrowCount + 1);
			duck.SwitchDuckState(DuckState.Flying, _duckConfigsService.DuckConfig.FlightSeconds);

			_entityFactory.Event()
				.With(x => x.isDuckThrownEvent = true);
		}
	}
}
