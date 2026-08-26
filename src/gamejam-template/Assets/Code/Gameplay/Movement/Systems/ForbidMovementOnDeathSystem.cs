using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class ForbidMovementOnDeathSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _deadMovers;

		private readonly List<GameEntity> _buffer = new(16);

		public ForbidMovementOnDeathSystem(GameContext game)
		{
			_deadMovers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Dead,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			foreach (GameEntity deadMover in _deadMovers.GetEntities(_buffer))
			{
				deadMover.isCanMove = false;
			}
		}
	}
}
