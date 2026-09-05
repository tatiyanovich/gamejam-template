using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Meow.Configs
{
	[CreateAssetMenu(fileName = "MeowConfig", menuName = "Configs/Meow/MeowConfig")]
	public class MeowConfig : ScriptableObject
	{
		[SF] private float levelScale = 420f;
		[SF] private float thresholdLevel = 40f;
		[SF] private float rearmLevel = 30f;
		[SF] private float cooldownSeconds = 0.7f;

		public float LevelScale => levelScale;
		public float ThresholdLevel => thresholdLevel;
		public float RearmLevel => rearmLevel;
		public float CooldownSeconds => cooldownSeconds;
	}
}
