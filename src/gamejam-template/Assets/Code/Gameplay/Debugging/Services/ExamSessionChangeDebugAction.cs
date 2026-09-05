using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Infrastructure.CoreLoop;
using UnityEngine;

namespace Code.Gameplay.Debugging.Services
{
    public class ExamSessionChangeDebugAction : IGameplayDebugInputAction
    {
        private readonly ICoreLoopRequestFactory _coreLoopRequestFactory;
        private readonly ICameraSwitcher _cameraSwitcher;

        public ExamSessionChangeDebugAction(
            ICoreLoopRequestFactory coreLoopRequestFactory, 
            ICameraSwitcher cameraSwitcher)
        {
            _coreLoopRequestFactory = coreLoopRequestFactory;
            _cameraSwitcher = cameraSwitcher;   
        }
        
        public bool WasTriggeredThisFrame() => UnityEngine.Input.GetKeyDown(KeyCode.Escape);

        public void Execute(Vector3 pointerWorldPosition)
        {
            _coreLoopRequestFactory.CreateCloseBranchRequest(LoopNodeId.Exam);
            _coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
            _cameraSwitcher.SwitchTo(LoopNodeId.Exam);
        }
    }
}