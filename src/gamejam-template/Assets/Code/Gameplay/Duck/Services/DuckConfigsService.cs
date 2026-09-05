using Code.Gameplay.Duck.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Duck.Services
{
	public class DuckConfigsService : IDuckConfigsService
	{
		private readonly IAssetsService _assets;

		private const string DuckConfigKey = "duck_config";

		public DuckConfig DuckConfig { get; private set; }

		public DuckConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			DuckConfig = _assets.Load<DuckConfig>(DuckConfigKey);
		}
	}
}
