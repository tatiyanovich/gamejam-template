using System.Threading;
using Code.Gameplay.Camera.Services;
using Code.Gameplay.CoreLoop.Services;
using Code.Infrastructure.CoreLoop;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Launch
{
    public class LaunchWindow : WindowBase
    {
        [SF] private Button startButton;

        private ICoreLoopRequestFactory _coreLoopRequestFactory;
        private ICameraSwitcher _cameraSwitch;
        private const float FadeInDuration = 0.3f;

        [Inject]
        public void Construct(
            ICoreLoopRequestFactory coreLoopRequestFactory,
            ICameraSwitcher cameraSwitch)
        {
            _coreLoopRequestFactory = coreLoopRequestFactory;
            _cameraSwitch = cameraSwitch;
        }
        
        protected override UniTask OnOpen(CancellationToken cancellationToken = default)
        {
            startButton.OnClicked += HandleStart;
            
            return base.OnOpen(cancellationToken);
        }
        
        protected override UniTask OnClose(CancellationToken cancellationToken = default)
        {
            startButton.OnClicked -= HandleStart;
            return base.OnClose(cancellationToken);
        }

        private void HandleStart() => FadeToBlackThenTransition().Forget();
        
        private async UniTaskVoid FadeToBlackThenTransition()
        {
            FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
            await fadeWindow.FadeIn(FadeInDuration);
            
            _cameraSwitch.SwitchTo(LoopNodeId.Exam);

            _coreLoopRequestFactory.CreateGoToBranchRequest(LoopNodeId.Exam);
        }
    }
}
