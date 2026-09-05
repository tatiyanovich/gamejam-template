using System.Collections.Generic;
using Code.Gameplay.Duck.Configs;
using Code.Gameplay.Duck.Services;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class LandThrownDuckSystem : IExecuteSystem
	{
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _timedDucks;

		private readonly List<GameEntity> _buffer = new(1);

		public LandThrownDuckSystem(GameContext game, IDuckConfigsService duckConfigsService)
		{
			_duckConfigsService = duckConfigsService;

			_timedDucks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckStateTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity duck in _timedDucks.GetEntities(_buffer))
			{
				if (duck.DuckState != DuckState.Flying)
					continue;

				if (duck.DuckStateTimeLeft > 0)
					continue;

				duck.SwitchDuckState(
					DuckState.OnFloor,
					RemainingDistractionSeconds(_duckConfigsService.DuckConfig));
			}
		}

		private static float RemainingDistractionSeconds(DuckConfig config)
		{
			return config.DistractionSeconds - config.FlightSeconds;
		}
	}
}
