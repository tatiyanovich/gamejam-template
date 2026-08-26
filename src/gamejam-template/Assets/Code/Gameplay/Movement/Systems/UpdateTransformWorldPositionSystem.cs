using Entitas;

namespace Code.Gameplay.Movement.Systems
{
    public class UpdateTransformWorldPositionSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _transforms;

        public UpdateTransformWorldPositionSystem(GameContext game)
        {
            _transforms = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.TransformMovement,
                    GameMatcher.WorldPosition,
                    GameMatcher.Transform)
                .AnyOf(
                    GameMatcher.WorldPositionChanged,
                    GameMatcher.ViewChanged));
        }

        public void Execute()
        {
            foreach (GameEntity mover in _transforms)
            {
                mover.Transform.position = mover.WorldPosition;
            }
        }
    }
}
