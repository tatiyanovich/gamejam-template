using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class RefreshVelocityWithRigidbody2DSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;
		private readonly List<GameEntity> _buffer = new(64);

		public RefreshVelocityWithRigidbody2DSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.RigidbodyVelocity2DMovement,
					GameMatcher.Rigidbody2D,
					GameMatcher.Velocity)
				.NoneOf(
					GameMatcher.VelocityChanged));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers.GetEntities(_buffer))
			{
				mover.ReplaceVelocity(mover.Rigidbody2D.linearVelocity);
			}
		}
	}
}
