using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Tutorial
{
	public class TutorialWindow : WindowBase
	{
		[SF] private Button dismissButton;

		private bool _isClosing;
		private Action _onDismissed;

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_isClosing = false;
			dismissButton.interactable = true;
			dismissButton.onClick.AddListener(HandleClose);
			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			dismissButton.onClick.RemoveListener(HandleClose);
			_onDismissed = null;
			return base.OnClose(cancellationToken);
		}

		public void Prepare(Action onDismissed)
		{
			_onDismissed = onDismissed;
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
