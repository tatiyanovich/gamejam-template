namespace Code.Gameplay.Leaderboard.Data
{
	public readonly struct LeaderboardEntry
	{
		public readonly string Name;
		public readonly int Answers;
		public readonly float TimeSeconds;
		public readonly string Grade;

		public LeaderboardEntry(string name, int answers, float timeSeconds, string grade)
		{
			Name = name;
			Answers = answers;
			TimeSeconds = timeSeconds;
			Grade = grade;
		}
	}
}
