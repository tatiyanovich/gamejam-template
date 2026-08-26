using Code.Infrastructure.CoreLoop;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Destruct
{
    [Game] public class Destructed : IComponent { }
    [Game] public class InactiveSceneEntity : IComponent { }

    [Game] public class PersistAcrossLoopNodes : IComponent { }

    // Identifies which loop node owns this entity, so a closing branch wipes only its own entities.
    [Game] public class LoopNodeScope : IComponent { public LoopNodeId NodeId; }
}