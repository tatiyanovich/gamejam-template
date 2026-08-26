using System.Threading;
using Code.Gameplay.Pickups.Queries;
using Code.UI.Result;
using Code.UI.Settings;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	// The in-game HUD. Views read the domain through queries and never touch entities:
	// score arrives as an event from ScoreQuery, buttons only open other windows.
	public class GameplayWindow : WindowBase
	{
		[SF] private TextMeshProUGUI scoreText;
		[SF] private Button settingsButton;
		[SF] private Button finishButton;

		private IScoreQuery _scoreQuery;

		[Inject]
		private void Construct(IScoreQuery scoreQuery)
		{
			_scoreQuery = scoreQuery;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_scoreQuery.OnScoreChanged += HandleScoreChanged;
			settingsButton.OnClicked += HandleSettingsClicked;
			finishButton.OnClicked += HandleFinishClicked;

			HandleScoreChanged(_scoreQuery.GetScore());

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			_scoreQuery.OnScoreChanged -= HandleScoreChanged;
			settingsButton.OnClicked -= HandleSettingsClicked;
			finishButton.OnClicked -= HandleFinishClicked;

			return base.OnClose(cancellationToken);
		}

		private void HandleScoreChanged(int score)
		{
			scoreText.text = score.ToString();
		}

		private void HandleSettingsClicked()
		{
			_uiService.OpenWindow<SettingsWindow>().Forget();
		}

		private void HandleFinishClicked()
		{
			_uiService.OpenWindow<ResultWindow>().Forget();
		}
	}
}
