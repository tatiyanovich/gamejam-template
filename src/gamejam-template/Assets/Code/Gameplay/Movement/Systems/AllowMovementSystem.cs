using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class AllowMovementSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		private readonly List<GameEntity> _buffer = new(16);

		public AllowMovementSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MovementSpeed,
					GameMatcher.Alive));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				mover.isCanMove = true;
			}
		}
	}
}
