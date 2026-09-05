using Code.Gameplay.Suspicion.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Suspicion.Services
{
	public class SuspicionConfigsService : ISuspicionConfigsService
	{
		private readonly IAssetsService _assets;

		private const string SuspicionConfigKey = "suspicion_config";

		public SuspicionConfig SuspicionConfig { get; private set; }

		public SuspicionConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			SuspicionConfig = _assets.Load<SuspicionConfig>(SuspicionConfigKey);
		}
	}
}
