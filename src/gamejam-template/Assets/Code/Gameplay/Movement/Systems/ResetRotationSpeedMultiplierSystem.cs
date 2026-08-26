using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class ResetRotationSpeedMultiplierSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _rotators;

		public ResetRotationSpeedMultiplierSystem(GameContext game)
		{
			_rotators = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RotationSpeedMultiplier));
		}

		public void Execute()
		{
			foreach (GameEntity rotator in _rotators)
			{
				rotator.ReplaceRotationSpeedMultiplier(1f);
			}
		}
	}
}
