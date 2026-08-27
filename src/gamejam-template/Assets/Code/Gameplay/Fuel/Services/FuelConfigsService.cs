using Code.Gameplay.Fuel.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Fuel.Services
{
	public class FuelConfigsService : IFuelConfigsService
	{
		private readonly IAssetsService _assets;

		private const string FuelConfigKey = "fuel_config";

		public FuelConfig FuelConfig { get; private set; }

		public FuelConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			FuelConfig = _assets.Load<FuelConfig>(FuelConfigKey);
		}
	}
}
