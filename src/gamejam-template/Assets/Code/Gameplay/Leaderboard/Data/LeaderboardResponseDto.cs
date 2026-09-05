using System;

namespace Code.Gameplay.Leaderboard.Data
{
	[Serializable]
	public class LeaderboardResponseDto
	{
		public LeaderboardEntryDto[] top;
		public int rank;
		public int total;
		public string error;

		public LeaderboardResponse ToResponse()
		{
			LeaderboardEntry[] entries = new LeaderboardEntry[top?.Length ?? 0];

			for (int index = 0; index < entries.Length; index++)
				entries[index] = top[index].ToEntry();

			return new LeaderboardResponse(
				top: entries,
				rank: rank,
				total: total,
				isOffline: false);
		}
	}
}
