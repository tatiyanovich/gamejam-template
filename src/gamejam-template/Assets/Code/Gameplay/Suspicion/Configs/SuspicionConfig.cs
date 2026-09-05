using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Suspicion.Configs
{
	[CreateAssetMenu(fileName = "SuspicionConfig", menuName = "Configs/Suspicion/SuspicionConfig")]
	public class SuspicionConfig : ScriptableObject
	{
		[SF] private float maximumLevel = 100f;
		[SF] private float watchedGainPerSecond = 35f;
		[SF] private float decayPerSecond = 5f;
		[SF] private float wrongInputPenalty = 8f;
		[SF] private float meowWhileWatchedPenalty = 15f;

		public float MaximumLevel => maximumLevel;
		public float WatchedGainPerSecond => watchedGainPerSecond;
		public float DecayPerSecond => decayPerSecond;
		public float WrongInputPenalty => wrongInputPenalty;
		public float MeowWhileWatchedPenalty => meowWhileWatchedPenalty;
	}
}
