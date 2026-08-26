using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Collisions.Systems
{
    public class CleanupCollisionsSystem : ICleanupSystem
    {
        private readonly IGroup<GameEntity> _collisions;
        private readonly List<GameEntity> _buffer = new(4);

        public CleanupCollisionsSystem(GameContext game)
        {
            _collisions = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Collision));
        }

        public void Cleanup()
        {
            foreach (GameEntity collision in _collisions.GetEntities(_buffer))
            {
                collision.RemoveCollisionComponents();
                collision.Destroy();
            }
        }
    }
}
