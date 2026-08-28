using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Error
{
	public class ErrorWindow : WindowBase
	{
		[SF] private TextMeshProUGUI logsText;
		[SF] private Button closeGameButton;

		protected override async UniTask OnOpen(CancellationToken cancellationToken = new())
		{
			closeGameButton.OnClicked += HandleCloseGameButtonClicked;

			await base.OnOpen(cancellationToken);
		}

		protected override async UniTask OnClose(CancellationToken cancellationToken = new())
		{
			closeGameButton.OnClicked -= HandleCloseGameButtonClicked;

			await base.OnClose(cancellationToken);
		}

		public void Setup(string recentLogs)
		{
			logsText.text = recentLogs;
		}

		private void HandleCloseGameButtonClicked()
		{
			Application.Quit();

#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
#endif
		}
	}
}
