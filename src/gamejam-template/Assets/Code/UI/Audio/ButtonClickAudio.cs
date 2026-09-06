using Code.Infrastructure.Audio;
using Code.Infrastructure.Audio.Services;
using UnityEngine;
using Zenject;
using FrameworkButton = Framework.UI.UiManagement.Elements.Buttons.Button;
using UnityButton = UnityEngine.UI.Button;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Audio
{
	[DisallowMultipleComponent]
	public class ButtonClickAudio : MonoBehaviour
	{
		[SF] private SfxId sound = SfxId.UiClick;

		private UnityButton _unityButton;
		private FrameworkButton _frameworkButton;

		private IAudioService _audioService;

		[Inject]
		private void Construct(IAudioService audioService)
		{
			_audioService = audioService;
		}

		private void Awake()
		{
			_unityButton = GetComponent<UnityButton>();
			_frameworkButton = GetComponent<FrameworkButton>();

			if (_unityButton != null)
				_unityButton.onClick.AddListener(HandleClicked);

			if (_frameworkButton != null)
				_frameworkButton.OnClicked += HandleClicked;
		}

		private void OnDestroy()
		{
			if (_unityButton != null)
				_unityButton.onClick.RemoveListener(HandleClicked);

			if (_frameworkButton != null)
				_frameworkButton.OnClicked -= HandleClicked;
		}

		public void Configure(SfxId value)
		{
			sound = value;
		}

		private void HandleClicked()
		{
			_audioService.PlaySfx(sound);
		}
	}
}
