using Entitas;
using UnityEngine;

namespace Code.Gameplay.Fuel.Systems
{
	public class StopMovementWithoutFuelSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _emptyTanks;

		public StopMovementWithoutFuelSystem(GameContext game)
		{
			_emptyTanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.FuelEmpty,
					GameMatcher.Velocity,
					GameMatcher.TargetVelocity));
		}

		public void Execute()
		{
			foreach (GameEntity emptyTank in _emptyTanks)
			{
				emptyTank.ReplaceVelocity(Vector3.zero);
				emptyTank.ReplaceTargetVelocity(Vector3.zero);
			}
		}
	}
}
