using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Fuel.Systems
{
	// The counterpart of ForbidMovementWithoutFuelSystem: a refuelled drill drives again.
	// Dead entities stay excluded so this never overrides ForbidMovementOnDeathSystem.
	public class AllowMovementWithFuelSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _refuelledTanks;

		private readonly List<GameEntity> _buffer = new(4);

		public AllowMovementWithFuelSystem(GameContext game)
		{
			_refuelledTanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Fuel,
					GameMatcher.MovementSpeed)
				.NoneOf(
					GameMatcher.FuelEmpty,
					GameMatcher.CanMove,
					GameMatcher.Dead));
		}

		public void Execute()
		{
			foreach (GameEntity refuelledTank in _refuelledTanks.GetEntities(_buffer))
			{
				refuelledTank.isCanMove = true;
			}
		}
	}
}
