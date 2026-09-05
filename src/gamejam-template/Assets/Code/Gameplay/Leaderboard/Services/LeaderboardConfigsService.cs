using Code.Gameplay.Leaderboard.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Leaderboard.Services
{
	public class LeaderboardConfigsService : ILeaderboardConfigsService
	{
		private readonly IAssetsService _assets;

		private const string LeaderboardConfigKey = "leaderboard_config";

		public LeaderboardConfig LeaderboardConfig { get; private set; }

		public LeaderboardConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			LeaderboardConfig = _assets.Load<LeaderboardConfig>(LeaderboardConfigKey);
		}
	}
}
