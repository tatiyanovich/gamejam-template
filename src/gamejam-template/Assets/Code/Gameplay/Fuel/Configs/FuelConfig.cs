using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Fuel.Configs
{
	[CreateAssetMenu(fileName = "FuelConfig", menuName = "Configs/FuelConfig")]
	public class FuelConfig : ScriptableObject
	{
		[SF, Min(0f)] private float maxFuel = 100f;
		[SF, Min(0f)] private float drainPerSecond = 4f;

		public float MaxFuel => maxFuel;
		public float DrainPerSecond => drainPerSecond;
	}
}
