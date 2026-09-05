using Code.Gameplay.Bell.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Bell.Services
{
	public class BellConfigsService : IBellConfigsService
	{
		private readonly IAssetsService _assets;

		private const string BellConfigKey = "bell_config";

		public BellConfig BellConfig { get; private set; }

		public BellConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			BellConfig = _assets.Load<BellConfig>(BellConfigKey);
		}
	}
}
