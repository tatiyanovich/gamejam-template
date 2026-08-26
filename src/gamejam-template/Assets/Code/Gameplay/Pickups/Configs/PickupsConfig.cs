using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Pickups.Configs
{
	[CreateAssetMenu(fileName = "PickupsConfig", menuName = "Configs/PickupsConfig")]
	public class PickupsConfig : ScriptableObject
	{
		[SF, Min(0)] private int spawnCount = 12;
		[SF, Min(0f)] private float spawnRadius = 12f;
		[SF, Min(0f)] private float minDistanceFromSpawn = 3f;
		[SF, Min(1)] private int scorePerPickup = 1;
		[SF, Min(0f)] private float collectRadius = 1.2f;

		public int SpawnCount => spawnCount;
		public float SpawnRadius => spawnRadius;
		public float MinDistanceFromSpawn => minDistanceFromSpawn;
		public int ScorePerPickup => scorePerPickup;
		public float CollectRadius => collectRadius;
	}
}
