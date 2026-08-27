using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Fuel.Systems
{
	public class MarkFuelEmptySystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _tanks;

		private readonly List<GameEntity> _buffer = new(4);

		public MarkFuelEmptySystem(GameContext game)
		{
			_tanks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Fuel));
		}

		public void Execute()
		{
			foreach (GameEntity tank in _tanks.GetEntities(_buffer))
			{
				tank.isFuelEmpty = tank.Fuel <= 0f;
			}
		}
	}
}
