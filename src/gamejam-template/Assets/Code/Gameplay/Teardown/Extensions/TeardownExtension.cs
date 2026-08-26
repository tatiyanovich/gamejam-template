using Code.UI;
using Code.UI.Gameplay;
using Code.UI.Joystick;
using Code.UI.Result;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Services;

namespace Code.Gameplay.Teardown.Extensions
{
    public static class TeardownExtension
    {
        public static async void CloseGameplayScreens(this IUiService uiService)
        {
            await UniTask.WhenAll(
                uiService.CloseWindow<GameplayWindow>(),
                uiService.CloseWindow<WorldOverlayWindow>(),
                uiService.CloseWindow<JoystickWindow>(),
                uiService.CloseWindow<ResultWindow>());
        }
    }
}