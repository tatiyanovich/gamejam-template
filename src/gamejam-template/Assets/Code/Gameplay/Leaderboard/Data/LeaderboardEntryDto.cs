using System;

namespace Code.Gameplay.Leaderboard.Data
{
	[Serializable]
	public class LeaderboardEntryDto
	{
		public string name;
		public int answers;
		public float timeSeconds;
		public string grade;

		public static LeaderboardEntryDto From(LeaderboardEntry entry)
		{
			return new LeaderboardEntryDto
			{
				name = entry.Name,
				answers = entry.Answers,
				timeSeconds = entry.TimeSeconds,
				grade = entry.Grade
			};
		}

		public LeaderboardEntry ToEntry() => new(name, answers, timeSeconds, grade);
	}
}
