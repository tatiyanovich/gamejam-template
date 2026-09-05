using Code.Gameplay.Leaderboard.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Leaderboard.Services
{
	public interface ILeaderboardConfigsService : IConfigsService
	{
		LeaderboardConfig LeaderboardConfig { get; }
	}
}
