using System.Threading;
using Code.Common.Utilities;
using Code.Infrastructure.Settings;
using Code.Infrastructure.Settings.Services;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Settings
{
	public class SettingsWindow : WindowBase
	{
		[SF] private GameObject performanceHint;
		[SF] private TMP_Text versionText;

		private ISettingsService _settingsService;

		[Inject]
		private void Construct(ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = new CancellationToken())
		{
			_settingsService.OnSettingChanged += HandleSettingChanged;
			RefreshPerformanceHint();
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = new CancellationToken())
		{
			_settingsService.OnSettingChanged -= HandleSettingChanged;

			return base.OnClose(cancellationToken);
		}

		private void HandleSettingChanged(SettingTypeId settingTypeId)
		{
			RefreshPerformanceHint();
		}

		private void RefreshPerformanceHint()
		{
			int currentFramerate = _settingsService.GetFramerateValue(_settingsService.Framerate);
			int middleFramerate = _settingsService.GetFramerateValue(FramerateTypeId.Mid);
			bool isHighQualityEnabled = _settingsService.IsEnabled(SettingTypeId.Quality);
			bool shouldShowHint = currentFramerate > middleFramerate || isHighQualityEnabled;

			performanceHint.SetActive(shouldShowHint);
		}

	}
}
