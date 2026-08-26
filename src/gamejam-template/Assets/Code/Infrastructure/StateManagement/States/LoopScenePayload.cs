using Code.Gameplay.CoreLoop;
using Code.Infrastructure.CoreLoop;

namespace Code.Infrastructure.StateManagement.States
{
    public struct LoopScenePayload
    {
        public LoopNodeId LoopNodeId;
        public string SceneAddress;

        public LoopScenePayload(LoopNodeId loopNodeId, string sceneAddress)
        {
            LoopNodeId = loopNodeId;
            SceneAddress = sceneAddress;
        }
    }
}