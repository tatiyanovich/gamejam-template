using System.Threading;
using Code.Gameplay.Fuel.Queries;
using Code.Gameplay.Pickups.Queries;
using Code.UI.Result;
using Code.UI.Settings;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Buttons;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using Zenject;
using Image = UnityEngine.UI.Image;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	// The in-game HUD. Views read the domain through queries and never touch entities:
	// score and fuel arrive as events from their queries, buttons only open other windows.
	public class GameplayWindow : WindowBase
	{
		[SF] private TextMeshProUGUI scoreText;
		[SF] private TextMeshProUGUI fuelText;
		[SF] private Image fuelFill;
		[SF] private Button settingsButton;
		[SF] private Button finishButton;

		private IScoreQuery _scoreQuery;
		private IFuelQuery _fuelQuery;

		[Inject]
		private void Construct(IScoreQuery scoreQuery, IFuelQuery fuelQuery)
		{
			_scoreQuery = scoreQuery;
			_fuelQuery = fuelQuery;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_scoreQuery.OnScoreChanged += HandleScoreChanged;
			_fuelQuery.OnFuelChanged += HandleFuelChanged;
			settingsButton.OnClicked += HandleSettingsClicked;
			finishButton.OnClicked += HandleFinishClicked;

			HandleScoreChanged(_scoreQuery.GetScore());
			HandleFuelChanged(_fuelQuery.GetFuel(), _fuelQuery.GetMaxFuel());

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			_scoreQuery.OnScoreChanged -= HandleScoreChanged;
			_fuelQuery.OnFuelChanged -= HandleFuelChanged;
			settingsButton.OnClicked -= HandleSettingsClicked;
			finishButton.OnClicked -= HandleFinishClicked;

			return base.OnClose(cancellationToken);
		}

		private void HandleScoreChanged(int score)
		{
			scoreText.text = score.ToString();
		}

		private void HandleFuelChanged(float fuel, float maxFuel)
		{
			// maxFuel is zero until the tank entity exists, which is the first frame of the node.
			float normalized = maxFuel <= 0f ? 0f : Mathf.Clamp01(fuel / maxFuel);

			fuelFill.fillAmount = normalized;
			fuelText.text = $"FUEL {Mathf.CeilToInt(normalized * 100f)}%";
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
