using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Duck.Systems
{
	public class ReturnDuckSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _timedDucks;

		private readonly List<GameEntity> _buffer = new(1);

		public ReturnDuckSystem(GameContext game)
		{
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
				if (duck.DuckState != DuckState.Carried)
					continue;

				if (duck.DuckStateTimeLeft > 0)
					continue;

				duck.SettleDuck(DuckState.OnDesk);
			}
		}
	}
}
