using Entitas;
using UnityEngine;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class MarkIsMovingSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public MarkIsMovingSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MovementDirection));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				mover.isMoving = mover.MovementDirection != Vector3.zero;
			}
		}
	}
}
