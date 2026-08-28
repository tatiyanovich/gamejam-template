using System.Threading;
using Code.Gameplay.Drilling.Queries;
using Code.Gameplay.Fuel.Queries;
using Code.UI.Result;
using Cysharp.Threading.Tasks;
using Framework.UI.UiManagement.Elements.Windows;
using TMPro;
using UnityEngine;
using Zenject;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Gameplay
{
	public class GameplayWindow : WindowBase
	{
		[SF] private TextMeshProUGUI fuelText;
		[SF] private ProgressBar fuelBar;

		private IDrillingQuery _drillingQuery;
		private IFuelQuery _fuelQuery;

		[Inject]
		private void Construct(IDrillingQuery drillingQuery, IFuelQuery fuelQuery)
		{
			_drillingQuery = drillingQuery;
			_fuelQuery = fuelQuery;
		}

		protected override UniTask OnOpen(CancellationToken cancellationToken = default)
		{
			_drillingQuery.OnRunFinished += HandleRunFinished;
			_fuelQuery.OnFuelChanged += HandleFuelChanged;

			HandleFuelChanged(_fuelQuery.GetFuel(), _fuelQuery.GetMaxFuel());

			return base.OnOpen(cancellationToken);
		}

		protected override UniTask OnClose(CancellationToken cancellationToken = default)
		{
			_drillingQuery.OnRunFinished -= HandleRunFinished;
			_fuelQuery.OnFuelChanged -= HandleFuelChanged;

			return base.OnClose(cancellationToken);
		}

		private void HandleFuelChanged(float fuel, float maxFuel)
		{
			float normalized = maxFuel <= 0f ? 0f : Mathf.Clamp01(fuel / maxFuel);

			fuelBar.DirectValue = normalized;
			fuelText.text = $"{Mathf.CeilToInt(fuel)}/{Mathf.CeilToInt(maxFuel)}";
		}

		private void HandleRunFinished()
		{
			_uiService.OpenWindow<ResultWindow>().Forget();
		}
	}
}
