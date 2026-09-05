using System.Collections.Generic;
using System.Threading;
using Code.Gameplay.Leaderboard.Data;
using Code.Gameplay.Leaderboard.Services;
using Cysharp.Threading.Tasks;

namespace Code.Editor
{
	public class PlaytestLeaderboard : ILeaderboardService
	{
		public LeaderboardResponse Response { get; set; } = LeaderboardResponse.Offline;
		public LeaderboardEntry Submitted { get; private set; }
		public int SubmitCount { get; private set; }
		public bool IsPending { get; set; }

		public async UniTask<LeaderboardResponse> Submit(
			LeaderboardEntry entry,
			CancellationToken cancellationToken = default)
		{
			Submitted = entry;
			SubmitCount++;

			while (IsPending)
				await UniTask.NextFrame(cancellationToken);

			return Response;
		}

		public static LeaderboardResponse BuildResponse(int rank, int total)
		{
			List<LeaderboardEntry> top = new(10);

			for (int index = 0; index < 10; index++)
				top.Add(new LeaderboardEntry($"Cat{index + 1}", 12 - index, 100f + index, "A"));

			return new LeaderboardResponse(top, rank, total, isOffline: false);
		}
	}
}
