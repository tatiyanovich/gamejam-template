using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class RefreshRigidbodyWithVelocitySystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public RefreshRigidbodyWithVelocitySystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Velocity,
					GameMatcher.VelocityChanged,
					GameMatcher.Rigidbody)
				.AnyOf(
					GameMatcher.RigidbodyVelocityMovement,
					GameMatcher.RigidbodyInterpolatedMovement));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				mover.Rigidbody.linearVelocity = mover.Velocity;
			}
		}
	}
}
