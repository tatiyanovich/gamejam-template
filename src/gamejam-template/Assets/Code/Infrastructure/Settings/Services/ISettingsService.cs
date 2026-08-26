using System;

namespace Code.Infrastructure.Settings.Services
{
	public interface ISettingsService
	{
		FramerateTypeId Framerate { get; }
		event Action<SettingTypeId> OnSettingChanged;
		void LoadProgress();
		bool IsEnabled(SettingTypeId typeId);
		void Toggle(SettingTypeId typeId, bool value);
		void SetTargetFramerate(FramerateTypeId framerateTypeId);
		int GetFramerateValue(FramerateTypeId framerateTypeId);
	}
}
