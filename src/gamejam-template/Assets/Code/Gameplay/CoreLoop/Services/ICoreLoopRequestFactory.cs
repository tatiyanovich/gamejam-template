using Code.Infrastructure.CoreLoop;

namespace Code.Gameplay.CoreLoop.Services
{
    public interface ICoreLoopRequestFactory
    {
        void CreateGoToNodeRequest(LoopNodeId loopNodeId);
        void CreateGoToBranchRequest(LoopNodeId loopNodeId);
        void CreateCloseBranchRequest(LoopNodeId loopNodeId);
    }
}
