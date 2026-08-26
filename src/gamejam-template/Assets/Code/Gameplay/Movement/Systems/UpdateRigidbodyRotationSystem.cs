using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class UpdateRigidbodyRotationSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _rotators;

		public UpdateRigidbodyRotationSystem(GameContext game)
		{
			_rotators = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.WorldRotation,
					GameMatcher.Rigidbody));
		}

		public void Execute()
		{
			foreach (GameEntity rotator in _rotators)
			{
				rotator.Rigidbody.MoveRotation(rotator.WorldRotation);
			}
		}
	}
}
