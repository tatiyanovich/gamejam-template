using System;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Fuel.Queries
{
	public class FuelQuery : IFuelQuery, IReactiveQuery
	{
		private readonly IGroup<GameEntity> _tanks;
		private readonly IGroup<GameEntity> _changedTanks;

		public event Action<float, float> OnFuelChanged;

		public FuelQuery(GameContext game)
		{
			_tanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Fuel,
					GameMatcher.MaxFuel));

			_changedTanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Fuel,
					GameMatcher.MaxFuel,
					GameMatcher.FuelChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity tank in _changedTanks)
			{
				OnFuelChanged?.Invoke(tank.Fuel, tank.MaxFuel);
			}
		}

		public float GetFuel()
		{
			foreach (GameEntity tank in _tanks)
				return tank.Fuel;

			return 0f;
		}

		public float GetMaxFuel()
		{
			foreach (GameEntity tank in _tanks)
				return tank.MaxFuel;

			return 0f;
		}
	}
}
