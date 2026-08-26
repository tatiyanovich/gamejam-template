using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class UpdateTransformLocalPositionSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _transforms;

		public UpdateTransformLocalPositionSystem(GameContext game)
		{
			_transforms = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.TransformMovement,
					GameMatcher.LocalPosition,
					GameMatcher.Transform));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _transforms)
			{
				mover.Transform.localPosition = mover.LocalPosition;
			}
		}
	}
}