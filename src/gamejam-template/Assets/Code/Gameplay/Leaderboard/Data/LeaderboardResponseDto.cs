using System;
using System.Collections.Generic;

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
			return new LeaderboardResponse(
				top: BestPerName(top),
				rank: rank,
				total: total,
				isOffline: false);
		}

		private static IReadOnlyList<LeaderboardEntry> BestPerName(LeaderboardEntryDto[] entries)
		{
			List<LeaderboardEntry> result = new(entries?.Length ?? 0);
			HashSet<string> seenNames = new();

			for (int index = 0; index < (entries?.Length ?? 0); index++)
			{
				LeaderboardEntryDto entry = entries[index];

				if (seenNames.Add(entry.name) == false)
					continue;

				result.Add(entry.ToEntry());
			}

			return result;
		}
	}
}
