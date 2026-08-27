using Code.Gameplay.Fuel.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Fuel.Systems
{
	// Every pickup is also a fuel can: that is what keeps a drained drill playable without
	// a separate refuel entity, and it closes the loop drive -> burn -> collect.
	public class RefuelOnPickupCollectedSystem : IExecuteSystem
	{
		private readonly IFuelConfigsService _fuelConfigsService;

		private readonly IGroup<GameEntity> _collectedEvents;
		private readonly IGroup<GameEntity> _tanks;

		public RefuelOnPickupCollectedSystem(GameContext game, IFuelConfigsService fuelConfigsService)
		{
			_fuelConfigsService = fuelConfigsService;

			_collectedEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.PickupCollectedEvent));

			_tanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Fuel,
					GameMatcher.MaxFuel));
		}

		public void Execute()
		{
			float refuel = _fuelConfigsService.FuelConfig.RefuelPerPickup;

			foreach (GameEntity collectedEvent in _collectedEvents)
			foreach (GameEntity tank in _tanks)
			{
				tank.ReplaceFuel(Mathf.Min(tank.MaxFuel, tank.Fuel + refuel));
			}
		}
	}
}
