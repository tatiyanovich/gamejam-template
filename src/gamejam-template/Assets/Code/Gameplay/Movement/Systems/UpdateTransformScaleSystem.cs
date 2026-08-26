using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class UpdateTransformScaleSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _transforms;

		public UpdateTransformScaleSystem(GameContext game)
		{
			_transforms = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.LossyScale,
					GameMatcher.Transform)
				.AnyOf(
					GameMatcher.LossyScaleChanged,
					GameMatcher.ViewChanged));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _transforms)
			{
				entity.Transform.localScale = entity.LossyScale;
			}
		}
	}
}
