using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Suspicion.Configs
{
	[CreateAssetMenu(fileName = "SuspicionConfig", menuName = "Configs/Suspicion/SuspicionConfig")]
	public class SuspicionConfig : ScriptableObject
	{
		[SF] private float maximumLevel = 100f;
		[SF] private float leanGainPerSecond = 12f;
		[SF] private float leanGainIncreasePerQuestion = 0.1f;
		[SF] private float decayPerSecond = 5f;
		[SF] private float wrongInputPenalty = 8f;
		[SF] private float meowWhileWatchedPenalty = 15f;

		public float MaximumLevel => maximumLevel;
		public float LeanGainPerSecond => leanGainPerSecond;
		public float LeanGainIncreasePerQuestion => leanGainIncreasePerQuestion;
		public float DecayPerSecond => decayPerSecond;
		public float WrongInputPenalty => wrongInputPenalty;
		public float MeowWhileWatchedPenalty => meowWhileWatchedPenalty;
	}
}
