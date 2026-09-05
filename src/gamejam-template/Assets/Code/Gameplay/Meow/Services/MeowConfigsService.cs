using Code.Gameplay.Meow.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Meow.Services
{
	public class MeowConfigsService : IMeowConfigsService
	{
		private readonly IAssetsService _assets;

		private const string MeowConfigKey = "meow_config";

		public MeowConfig MeowConfig { get; private set; }

		public MeowConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			MeowConfig = _assets.Load<MeowConfig>(MeowConfigKey);
		}
	}
}
