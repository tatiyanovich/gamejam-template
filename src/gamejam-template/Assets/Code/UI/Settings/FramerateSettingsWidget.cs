using System.Threading;
using Code.Infrastructure.Settings;
using Code.Infrastructure.Settings.Services;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Settings
{
	public class FramerateSettingsWidget : WidgetBase
	{
		[SF] private Button button;
		[SF] private TMP_Text framerateText;
		
		private ISettingsService _settingsService;

		[Inject]
		private void Construct(ISettingsService settingsService)
		{
			_settingsService = settingsService;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = new CancellationToken())
		{
			button.OnClicked += HandleButtonClicked;
			Setup();
			
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = new CancellationToken())
		{
			button.OnClicked -= HandleButtonClicked;
			return base.OnClose(cancellationToken);
		}

		private void Setup()
		{
			SetFramerateText(_settingsService.Framerate);
		}

		private void HandleButtonClicked()
		{
			FramerateTypeId nextFramerate = GetNextFramerateType(); 
			_settingsService.SetTargetFramerate(nextFramerate);
			SetFramerateText(nextFramerate);
		}

		private void SetFramerateText(FramerateTypeId framerateTypeId)
		{
			int nextFramerate = _settingsService.GetFramerateValue(framerateTypeId);
			framerateText.text = Constants.GraphicsQuality.MaxFramerate == nextFramerate 
				? "MAX" 
				: $"{nextFramerate}";
		}
		
		private FramerateTypeId GetNextFramerateType()
		{
			return _settingsService.Framerate switch
			{
				FramerateTypeId.Low => FramerateTypeId.Mid,
				FramerateTypeId.Mid => FramerateTypeId.Max,
				FramerateTypeId.Max => FramerateTypeId.Low,
				_ => throw new System.ArgumentOutOfRangeException(nameof(_settingsService.Framerate), _settingsService.Framerate, null)
			};
		}
	}
}