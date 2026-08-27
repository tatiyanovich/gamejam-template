using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Fuel.Systems
{
	public class ForbidMovementWithoutFuelSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _emptyTanks;

		private readonly List<GameEntity> _buffer = new(4);

		public ForbidMovementWithoutFuelSystem(GameContext game)
		{
			_emptyTanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.FuelEmpty,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			foreach (GameEntity emptyTank in _emptyTanks.GetEntities(_buffer))
			{
				emptyTank.isCanMove = false;
			}
		}
	}
}
