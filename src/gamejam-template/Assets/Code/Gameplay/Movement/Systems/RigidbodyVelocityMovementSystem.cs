using Entitas;

namespace Code.Gameplay.Movement.Systems
{
    public class RigidbodyVelocityMovementSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _movers;

        public RigidbodyVelocityMovementSystem(GameContext game)
        {
            _movers = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.RigidbodyVelocityMovement,
                    GameMatcher.MovementDirection,
                    GameMatcher.MovementSpeed,
                    GameMatcher.CanMove,
                    GameMatcher.Velocity));
        }

        public void Execute()
        {
            foreach (GameEntity mover in _movers)
            {
                mover.ReplaceVelocity(mover.MovementDirection * mover.MovementSpeed);
            }
        }
    }
}
