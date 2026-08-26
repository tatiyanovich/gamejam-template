using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class RigidbodyPositionMovementSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public RigidbodyPositionMovementSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RigidbodyPositionMovement,
					GameMatcher.Rigidbody,
					GameMatcher.WorldPosition));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				mover.Rigidbody.position = mover.WorldPosition;
			}
		}
	}
}
