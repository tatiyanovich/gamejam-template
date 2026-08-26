using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Camera.Configs
{
	[CreateAssetMenu(fileName = "ShakeConfig", menuName = "Configs/ShakeConfig")]
	public class ShakeConfig : ScriptableObject
	{
		[SF] private CameraShakeTypeId shakeTypeId;
		[SF] private float duration = 0.2f;
		[SF] private float frequency = 25f;
		[SF] private Vector3 positionAmplitude = new(0.35f, 0.35f, 0f);
		[SF] private Vector3 rotationAmplitude = new(0f, 0f, 1.25f);
		[SF] private AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public CameraShakeTypeId ShakeTypeId => shakeTypeId;
		public float Duration => duration;
		public float Frequency => frequency;
		public Vector3 PositionAmplitude => positionAmplitude;
		public Vector3 RotationAmplitude => rotationAmplitude;

		public AnimationCurve IntensityCurve
		{
			get
			{
				if (intensityCurve == null || intensityCurve.length == 0)
					intensityCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

				return intensityCurve;
			}
		}
	}
}
