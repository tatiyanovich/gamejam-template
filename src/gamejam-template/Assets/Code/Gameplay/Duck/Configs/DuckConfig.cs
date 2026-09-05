using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Duck.Configs
{
	[CreateAssetMenu(fileName = "DuckConfig", menuName = "Configs/Duck/DuckConfig")]
	public class DuckConfig : ScriptableObject
	{
		[SF] private float flightSeconds = 0.6f;
		[SF] private float distractionSeconds = 4f;
		[SF] private float returnSeconds = 8f;
		[SF] private float suspicionRelief = 20f;
		[SF] private int throwLimit = 3;

		public float FlightSeconds => flightSeconds;
		public float DistractionSeconds => distractionSeconds;
		public float ReturnSeconds => returnSeconds;
		public float SuspicionRelief => suspicionRelief;
		public int ThrowLimit => throwLimit;
	}
}
