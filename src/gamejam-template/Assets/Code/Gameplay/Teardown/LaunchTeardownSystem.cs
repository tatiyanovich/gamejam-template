using Code.UI.Launch;
using Entitas;
using Framework.UI.UiManagement.Services;

namespace Code.Gameplay.Teardown
{
    public class LaunchTeardownSystem : ITearDownSystem
    {
        private readonly IUiService _uiService;

        public LaunchTeardownSystem(IUiService uiService)
        {
            _uiService = uiService;
        }
        
        public void TearDown()
        {
            LaunchWindow launchWindow = _uiService.GetWindow<LaunchWindow>();
            
            if (launchWindow != null)
                _uiService.CloseWindow(launchWindow);
        }
    }
}