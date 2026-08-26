using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class TransformMovementSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public TransformMovementSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.TransformMovement,
					GameMatcher.WorldPosition,
					GameMatcher.MovementStep,
					GameMatcher.CanMove));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				mover.ReplacePreviousWorldPosition(mover.WorldPosition);
				mover.ReplaceWorldPosition(mover.WorldPosition + mover.MovementStep);
			}
		}
	}
}
