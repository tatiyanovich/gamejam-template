using System.Threading;
using Code.Gameplay.Leaderboard.Data;
using Cysharp.Threading.Tasks;

namespace Code.Gameplay.Leaderboard.Services
{
	public interface ILeaderboardService
	{
		UniTask<LeaderboardResponse> Submit(LeaderboardEntry entry, CancellationToken cancellationToken = default);
	}
}
