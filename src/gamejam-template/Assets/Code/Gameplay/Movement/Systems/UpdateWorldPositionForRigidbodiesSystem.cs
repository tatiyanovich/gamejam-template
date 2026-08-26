using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class UpdateWorldPositionForRigidbodiesSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _group;

		public UpdateWorldPositionForRigidbodiesSystem(GameContext game)
		{
			_group = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Rigidbody,
					GameMatcher.WorldPosition)
				.AnyOf(
					GameMatcher.RigidbodyVelocityMovement,
					GameMatcher.RigidbodyPositionMovement,
					GameMatcher.RigidbodyInterpolatedMovement));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _group)
			{
				if (mover.isWorldPositionChanged)
				{
					mover.Rigidbody.position = mover.WorldPosition;

					if (mover.hasTransform)
						mover.Transform.position = mover.WorldPosition;
				}
				else
				{
					mover.ReplaceWorldPosition(mover.Rigidbody.position);
				}
			}
		}
	}
}
