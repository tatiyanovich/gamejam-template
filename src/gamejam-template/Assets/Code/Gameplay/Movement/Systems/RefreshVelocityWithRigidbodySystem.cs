using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public class RefreshVelocityWithRigidbodySystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;
		private readonly List<GameEntity> _buffer = new(64);

		public RefreshVelocityWithRigidbodySystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Velocity,
					GameMatcher.Rigidbody)
				.AnyOf(
					GameMatcher.RigidbodyInterpolatedMovement,
					GameMatcher.RigidbodyPositionMovement,
					GameMatcher.RigidbodyVelocityMovement)
				.NoneOf(
					GameMatcher.VelocityChanged));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				mover.ReplaceVelocity(mover.Rigidbody.linearVelocity);
			}
		}
	}
}
