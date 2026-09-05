using Code.Gameplay.Leaderboard.Configs;
using Code.Gameplay.Leaderboard.Services;

namespace Code.Editor
{
	public class PlaytestLeaderboardConfigs : ILeaderboardConfigsService
	{
		public LeaderboardConfig LeaderboardConfig { get; }

		public PlaytestLeaderboardConfigs(LeaderboardConfig config)
		{
			LeaderboardConfig = config;
		}

		public void LoadConfigs() { }
	}
}
