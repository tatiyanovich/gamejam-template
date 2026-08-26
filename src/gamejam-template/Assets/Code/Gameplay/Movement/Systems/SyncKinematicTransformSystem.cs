using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class SyncKinematicTransformSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public SyncKinematicTransformSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.KinematicMovement,
					GameMatcher.WorldPosition,
					GameMatcher.Transform)
				.AnyOf(
					GameMatcher.WorldPositionChanged,
					GameMatcher.ViewChanged));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				mover.Transform.position = mover.WorldPosition;
			}
		}
	}
}
