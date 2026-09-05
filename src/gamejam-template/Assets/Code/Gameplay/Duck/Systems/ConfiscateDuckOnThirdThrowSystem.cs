using System.Collections.Generic;
using Code.Gameplay.Duck.Services;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class ConfiscateDuckOnThirdThrowSystem : IExecuteSystem
	{
		private readonly IDuckConfigsService _duckConfigsService;

		private readonly IGroup<GameEntity> _timedDucks;

		private readonly List<GameEntity> _buffer = new(1);

		public ConfiscateDuckOnThirdThrowSystem(GameContext game, IDuckConfigsService duckConfigsService)
		{
			_duckConfigsService = duckConfigsService;

			_timedDucks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckStateTimeLeft,
					GameMatcher.DuckThrowCount));
		}

		public void Execute()
		{
			foreach (GameEntity duck in _timedDucks.GetEntities(_buffer))
			{
				if (duck.DuckState != DuckState.OnFloor)
					continue;

				if (duck.DuckStateTimeLeft > 0)
					continue;

				if (duck.DuckThrowCount < _duckConfigsService.DuckConfig.ThrowLimit)
					continue;

				duck.SettleDuck(DuckState.Confiscated);
			}
		}
	}
}
