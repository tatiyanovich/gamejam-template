using Code.Gameplay.Leaderboard.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SF = UnityEngine.SerializeField;

namespace Code.UI.Result
{
	public class ResultLeaderboardRow : MonoBehaviour
	{
		[SF] private Image highlight;
		[SF] private TMP_Text rank;
		[SF] private TMP_Text playerName;
		[SF] private TMP_Text answers;
		[SF] private TMP_Text time;
		[SF] private TMP_Text grade;

		public void Show(LeaderboardEntry entry, int rankNumber, bool isOwn)
		{
			rank.text = rankNumber.ToString();
			playerName.text = entry.Name;
			answers.text = entry.Answers.ToString();
			time.text = ResultTimeFormat.Format(entry.TimeSeconds);
			grade.text = entry.Grade;
			highlight.enabled = isOwn;
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}
