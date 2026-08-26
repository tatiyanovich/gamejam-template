using Code.Gameplay.Player.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Player.Services
{
	public class PlayerConfigsService : IPlayerConfigsService
	{
		private readonly IAssetsService _assets;

		private const string PlayerConfigKey = "player_config";

		public PlayerConfig PlayerConfig { get; private set; }

		public PlayerConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			PlayerConfig = _assets.Load<PlayerConfig>(PlayerConfigKey);
		}
	}
}
