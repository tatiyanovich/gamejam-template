using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class UpdateTransformRotationSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _transforms;

		public UpdateTransformRotationSystem(GameContext game)
		{
			_transforms = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.WorldRotation,
					GameMatcher.Transform)
				.NoneOf(
					GameMatcher.Rigidbody, GameMatcher.Rigidbody2D));
		}

		public void Execute()
		{
			foreach (GameEntity transformer in _transforms)
			{
				transformer.Transform.rotation = transformer.WorldRotation;
			}
		}
	}
}
