using System.Collections.Generic;
using Code.Gameplay.Duck.Services;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class PickUpDuckSystem : IExecuteSystem
	{
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _timedDucks;

		private readonly List<GameEntity> _buffer = new(1);

		public PickUpDuckSystem(GameContext game, IDuckConfigsService duckConfigsService)
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
				if (duck.DuckState != DuckState.OnFloor)
					continue;

				if (duck.DuckStateTimeLeft > 0)
					continue;

				duck.SwitchDuckState(DuckState.Carried, _duckConfigsService.DuckConfig.ReturnSeconds);
			}
		}
	}
}
