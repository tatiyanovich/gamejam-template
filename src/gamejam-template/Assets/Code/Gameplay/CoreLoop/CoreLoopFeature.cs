using Code.Gameplay.CoreLoop.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.CoreLoop
{
    public class CoreLoopFeature : Feature
    {
        public CoreLoopFeature(ISystemFactory systems)
        {
            Add(systems.Create<HandleLoopNodeRequestSystem>());
            // Close before open so replaying the same node (close Battle + open Battle in one frame)
            // tears the branch down first, then re-opens it — otherwise open no-ops on the still-live
            // branch and the following close leaves nothing running.
            Add(systems.Create<HandleCloseBranchRequestSystem>());
            Add(systems.Create<HandleNewBranchRequestSystem>());
        }
    }
}