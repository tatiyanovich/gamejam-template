using Entitas;
using Framework.Essentials.TimeManagement;
using UnityEngine;

namespace Code.Gameplay.Fuel.Systems
{
	public class DrainFuelWhileMovingSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _movingTanks;

		public DrainFuelWhileMovingSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_movingTanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Fuel,
					GameMatcher.FuelDrainRate,
					GameMatcher.Moving,
					GameMatcher.CanMove)
				.NoneOf(
					GameMatcher.FuelEmpty));
		}

		public void Execute()
		{
			float deltaTime = _timeService.DeltaTime;

			foreach (GameEntity tank in _movingTanks)
			{
				float drained = tank.Fuel - tank.FuelDrainRate * deltaTime;

				tank.ReplaceFuel(Mathf.Max(0f, drained));
			}
		}
	}
}
