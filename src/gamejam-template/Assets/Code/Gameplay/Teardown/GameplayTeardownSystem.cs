using Code.Gameplay.Teardown.Extensions;
using Entitas;
using Framework.UI.UiManagement.Services;

namespace Code.Gameplay.Teardown
{
    public class GameplayTeardownSystem : ITearDownSystem
    {
        private readonly IUiService _uiService;

        public GameplayTeardownSystem(IUiService uiService)
        {
            _uiService = uiService;
        }
        
        public void TearDown()
        {
            _uiService.CloseGameplayScreens();
        }
    }
}