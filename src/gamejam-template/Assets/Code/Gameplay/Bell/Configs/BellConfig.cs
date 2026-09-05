using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Bell.Configs
{
	[CreateAssetMenu(fileName = "BellConfig", menuName = "Configs/Bell/BellConfig")]
	public class BellConfig : ScriptableObject
	{
		[SF] private float examSeconds = 120f;
		[SF] private float announcementSecondsLeft = 45f;

		public float ExamSeconds => examSeconds;
		public float AnnouncementSecondsLeft => announcementSecondsLeft;
	}
}
