using System;
using System.Threading;
using Code.Gameplay.Progress.Services;
using Code.Infrastructure.Input;
using Code.Infrastructure.Settings;
using Code.Infrastructure.Settings.Services;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Intro
{
	public class IntroWindow : WindowBase
	{
		[SF] private RawImage videoImage;
		[SF] private Button dismissButton;
		[SF] private VideoPlayer videoPlayer;
		[SF] private AudioSource audioSource;

		private bool _isClosing;
		private Action _onDismissed;
		private RenderTexture _videoTexture;

		private IInputService _inputService;
		private ISettingsService _settingsService;
		private IProgressFactory _progressFactory;

		[Inject]
		public void Construct(
			IInputService inputService,
			ISettingsService settingsService,
			IProgressFactory progressFactory)
		{
			_inputService = inputService;
			_settingsService = settingsService;
			_progressFactory = progressFactory;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_isClosing = false;
			dismissButton.interactable = true;
			dismissButton.onClick.AddListener(HandleDismiss);
			videoPlayer.loopPointReached += HandleVideoFinished;
			videoPlayer.errorReceived += HandleVideoError;
			_progressFactory.CreateMarkIntroSeenRequest();
			PlayVideo(cancellationToken).Forget();
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			dismissButton.onClick.RemoveListener(HandleDismiss);
			videoPlayer.loopPointReached -= HandleVideoFinished;
			videoPlayer.errorReceived -= HandleVideoError;
			videoPlayer.Stop();
			videoPlayer.targetTexture = null;
			videoImage.texture = null;
			ReleaseVideoTexture();
			_onDismissed = null;
			return base.OnClose(cancellationToken);
		}

		protected override void OnUpdate()
		{
			if (_inputService.IsKeyPressed(KeyCode.Escape) || _inputService.IsKeyPressed(KeyCode.Space))
				Dismiss();
		}

		public void Prepare(Action onDismissed)
		{
			_onDismissed = onDismissed;
		}

		private async UniTaskVoid PlayVideo(CancellationToken cancellationToken)
		{
			CreateVideoTexture();
			audioSource.mute = _settingsService.IsEnabled(SettingTypeId.Effects) == false;
			videoPlayer.Prepare();
			bool isCanceled = await UniTask
				.WaitUntil(() => videoPlayer.isPrepared, cancellationToken: cancellationToken)
				.SuppressCancellationThrow();

			if (isCanceled || _isClosing)
				return;

			videoPlayer.Play();
		}

		private void CreateVideoTexture()
		{
			int width = Mathf.Max(1, (int)videoPlayer.clip.width);
			int height = Mathf.Max(1, (int)videoPlayer.clip.height);
			_videoTexture = new RenderTexture(width, height, 0, RenderTextureFormat.Default);
			_videoTexture.name = "IntroVideo";
			_videoTexture.Create();
			videoPlayer.targetTexture = _videoTexture;
			videoImage.texture = _videoTexture;
		}

		private void ReleaseVideoTexture()
		{
			if (_videoTexture == null)
				return;

			_videoTexture.Release();
			Destroy(_videoTexture);
			_videoTexture = null;
		}

		private async UniTaskVoid Close()
		{
			Action onDismissed = _onDismissed;
			_onDismissed = null;
			await _uiService.CloseWindow<IntroWindow>();
			onDismissed?.Invoke();
		}

		private void Dismiss()
		{
			if (_isClosing)
				return;

			_isClosing = true;
			dismissButton.interactable = false;
			Close().Forget();
		}

		private void HandleDismiss()
		{
			Dismiss();
		}

		private void HandleVideoFinished(VideoPlayer source)
		{
			Dismiss();
		}

		private void HandleVideoError(VideoPlayer source, string message)
		{
			Debug.LogError($"Intro video playback failed: {message}");
			Dismiss();
		}
	}
}
