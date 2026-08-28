using System.Collections.Generic;
using Code.Gameplay.Fuel.Configs;
using Code.Gameplay.Fuel.Services;
using Entitas;

namespace Code.Gameplay.Fuel.Systems
{
	public class InitializePlayerFuelSystem : IExecuteSystem
	{
		private readonly IFuelConfigsService _fuelConfigsService;

		private readonly IGroup<GameEntity> _playersWithoutFuel;

		private readonly List<GameEntity> _buffer = new(4);

		public InitializePlayerFuelSystem(GameContext game, IFuelConfigsService fuelConfigsService)
		{
			_fuelConfigsService = fuelConfigsService;

			_playersWithoutFuel = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player)
				.NoneOf(
					GameMatcher.Fuel));
		}

		public void Execute()
		{
			FuelConfig fuelConfig = _fuelConfigsService.FuelConfig;

			foreach (GameEntity player in _playersWithoutFuel.GetEntities(_buffer))
			{
				player.AddFuel(fuelConfig.MaxFuel);
				player.AddMaxFuel(fuelConfig.MaxFuel);
				player.AddFuelDrainRate(fuelConfig.DrainPerSecond);
			}
		}
	}
}
