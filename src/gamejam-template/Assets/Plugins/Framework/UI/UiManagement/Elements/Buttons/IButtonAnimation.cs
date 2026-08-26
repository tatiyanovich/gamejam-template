using System;

namespace Framework.UI.UiManagement.Elements.Buttons
{
	public interface IButtonAnimation
	{
		void Initialize(Button button);
		void PlayPressAnimation(Action onPress);
		void PlayReleaseAnimation(Action onRelease, Action onClicked);
		void SetInteractable(bool state);
	}
}
