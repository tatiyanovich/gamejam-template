using Code.Gameplay.Pickups.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Pickups.Services
{
	public class PickupConfigsService : IPickupConfigsService
	{
		private readonly IAssetsService _assets;

		private const string PickupsConfigKey = "pickups_config";

		public PickupsConfig PickupsConfig { get; private set; }

		public PickupConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			PickupsConfig = _assets.Load<PickupsConfig>(PickupsConfigKey);
		}
	}
}
