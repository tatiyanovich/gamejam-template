using System.Collections.Generic;
using System.Threading;
using Code.Infrastructure.Settings;
using Code.Infrastructure.Settings.Services;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Settings
{
	public class SettingWidget : WidgetBase
	{
		[SF] private SettingTypeId typeId;
		[SF] private Button button;
		
		[SF] private List<GameObject> onStateObjects;
		[SF] private List<GameObject> offStateObjects;

		private ISettingsService _settingsService;

		[Inject]
		private void Construct(ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = new CancellationToken())
		{
			button.OnClicked += HandleClicked;
			_settingsService.OnSettingChanged += HandleSettingChanged;

			Setup();

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = new CancellationToken())
		{
			button.OnClicked -= HandleClicked;
			_settingsService.OnSettingChanged -= HandleSettingChanged;

			return base.OnClose(cancellationToken);
		}

		private void Setup()
		{
			bool isOn = _settingsService.IsEnabled(typeId);

			foreach (GameObject target in onStateObjects)
			{
				target.SetActive(isOn);
			}

			foreach (GameObject target in offStateObjects)
			{
				target.SetActive(isOn == false);
			}
		}

		private void HandleClicked()
		{
			_settingsService.Toggle(typeId, _settingsService.IsEnabled(typeId) == false);
		}

		private void HandleSettingChanged(SettingTypeId changedTypeId)
		{
			if (changedTypeId == typeId)
			{
				Setup();
			}
		}
	}
}
