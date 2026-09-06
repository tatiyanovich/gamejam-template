using System;
using System.Threading;
using Code.Infrastructure.Audio;
using Code.Infrastructure.Audio.Services;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine.UI;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Tutorial
{
	public class TutorialWindow : WindowBase
	{
		[SF] private Button dismissButton;

		private bool _isClosing;
		private bool _playVoiceOver;
		private Action _onDismissed;
		private CancellationTokenSource _voiceOverCts;

		private IAudioService _audioService;

		[Inject]
		public void Construct(IAudioService audioService)
		{
			_audioService = audioService;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_isClosing = false;
			dismissButton.interactable = true;
			dismissButton.onClick.AddListener(HandleClose);

			if (_playVoiceOver)
			{
				_voiceOverCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
				PlayIntroVoiceOver(_voiceOverCts.Token).Forget();
			}

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			dismissButton.onClick.RemoveListener(HandleClose);
			StopVoiceOver();
			_onDismissed = null;
			_playVoiceOver = false;
			return base.OnClose(cancellationToken);
		}

		public void Prepare(Action onDismissed, bool playVoiceOver = false)
		{
			_onDismissed = onDismissed;
			_playVoiceOver = playVoiceOver;
		}

		private async UniTaskVoid PlayIntroVoiceOver(CancellationToken cancellationToken)
		{
			VoiceOverId[] sequence =
			{
				VoiceOverId.IntroPanelOne,
				VoiceOverId.IntroPanelTwo,
				VoiceOverId.IntroPanelThree,
				VoiceOverId.IntroPanelFour
			};

			foreach (VoiceOverId voiceOverId in sequence)
			{
				bool isCanceled = await _audioService
					.PlayVoiceOver(voiceOverId, cancellationToken)
					.SuppressCancellationThrow();

				if (isCanceled)
					return;
			}
		}

		private void StopVoiceOver()
		{
			if (_voiceOverCts != null)
			{
				_voiceOverCts.Cancel();
				_voiceOverCts.Dispose();
				_voiceOverCts = null;
			}

			_audioService.StopVoiceOver();
		}

		private async UniTaskVoid Close()
		{
			Action onDismissed = _onDismissed;
			_onDismissed = null;
			await _uiService.CloseWindow<TutorialWindow>();
			onDismissed?.Invoke();
		}

		private void HandleClose()
		{
			if (_isClosing)
				return;

			_isClosing = true;
			dismissButton.interactable = false;
			Close().Forget();
		}
	}
}
