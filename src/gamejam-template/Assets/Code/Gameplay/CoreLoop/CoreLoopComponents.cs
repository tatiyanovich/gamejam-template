using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.StateManagement.States;
using Entitas;

namespace Code.Gameplay.CoreLoop
{
    [Game]
    public class GoToLoopNodeRequest : IComponent
    {
        public LoopScenePayload NodeId;
    }

    [Game]
    public class GoToBranchRequest : IComponent
    {
        public LoopScenePayload NodeId;
    }
    
    [Game]
    public class CloseBranchRequest : IComponent
    {
        public LoopNodeId NodeId;
    }
}