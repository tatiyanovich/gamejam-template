using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Leaderboard.Configs
{
	[CreateAssetMenu(fileName = "LeaderboardConfig", menuName = "Configs/Leaderboard/LeaderboardConfig")]
	public class LeaderboardConfig : ScriptableObject
	{
		[SF] private string url;
		[SF] private int requestTimeoutSeconds = 5;

		public string Url => url;
		public int RequestTimeoutSeconds => requestTimeoutSeconds;
	}
}
