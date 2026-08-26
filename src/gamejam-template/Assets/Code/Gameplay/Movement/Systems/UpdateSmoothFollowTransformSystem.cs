using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class UpdateSmoothFollowTransformSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _transforms;

		public UpdateSmoothFollowTransformSystem(GameContext game)
		{
			_transforms = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.WorldPosition,
					GameMatcher.Transform,
					GameMatcher.SmoothFollowMovement));
		}

		public void Execute()
		{
			foreach (GameEntity transformer in _transforms)
			{
				transformer.Transform.position = transformer.WorldPosition;
			}
		}
	}
}