using Entitas;

namespace Code.Gameplay.Movement.Systems
{
    public class UpdateTransformWorldPositionLateSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _transforms;

        public UpdateTransformWorldPositionLateSystem(GameContext game)
        {
            _transforms = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.WorldPosition,
                    GameMatcher.Transform,
                    GameMatcher.TransformLateMovement));
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
