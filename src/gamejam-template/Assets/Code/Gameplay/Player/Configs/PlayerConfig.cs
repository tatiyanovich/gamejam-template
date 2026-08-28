using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Player.Configs
{
	[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
	public class PlayerConfig : ScriptableObject
	{
		[SF, Min(0f)] private float moveSpeed = 8f;
		[SF, Min(0f)] private float maxMoveSpeed = 12f;
		[SF, Min(0f)] private float moveAcceleration = 40f;

		[Header("Kinematic")]
		[SF, Min(0f)] private float rotationSpeed = 720f;
		[SF, Range(0f, 1f)] private float moveDeadzone = 0.1f;
		[SF] private LayerMask obstacleMask = ~0;

		public float MoveSpeed => moveSpeed;
		public float MaxMoveSpeed => maxMoveSpeed;
		public float MoveAcceleration => moveAcceleration;

		public float RotationSpeed => rotationSpeed;
		public float MoveDeadzone => moveDeadzone;
		public LayerMask ObstacleMask => obstacleMask;
	}
}
