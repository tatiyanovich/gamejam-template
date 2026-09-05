using System;
using System.Collections.Generic;

namespace Code.Gameplay.Leaderboard.Data
{
	public readonly struct LeaderboardResponse
	{
		public readonly IReadOnlyList<LeaderboardEntry> Top;
		public readonly int Rank;
		public readonly int Total;
		public readonly bool IsOffline;

		public static LeaderboardResponse Offline => new(
			top: Array.Empty<LeaderboardEntry>(),
			rank: 0,
			total: 0,
			isOffline: true);

		public LeaderboardResponse(IReadOnlyList<LeaderboardEntry> top, int rank, int total, bool isOffline)
		{
			Top = top;
			Rank = rank;
			Total = total;
			IsOffline = isOffline;
		}
	}
}
