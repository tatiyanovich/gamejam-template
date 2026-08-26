using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Camera.Configs
{
	[CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/CameraConfig")]
	public class CameraConfig : ScriptableObject
	{
		[SF] private Vector3 offset;
		[SF] private float smoothing;
		[SF] private Vector3 rotation;

		public Vector3 Offset => offset;
		public float Smoothing => smoothing;
		public Vector3 Rotation => rotation;
	}
}
