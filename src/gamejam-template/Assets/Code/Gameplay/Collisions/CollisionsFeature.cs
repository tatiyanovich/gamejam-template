using Code.Gameplay.Collisions.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Collisions
{
    public sealed class CollisionsFeature : Feature
    {
        public CollisionsFeature(ISystemFactory systems)
        {
            Add(systems.Create<CleanupCollisionsSystem>());
        }
    }
}