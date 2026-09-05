using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Teacher.Configs
{
	[CreateAssetMenu(fileName = "TeacherConfig", menuName = "Configs/Teacher/TeacherConfig")]
	public class TeacherConfig : ScriptableObject
	{
		[SF] private float turningSeconds = 0.3f;
		[SF] private float alertDelayMinimum = 0.8f;
		[SF] private float alertDelayMaximum = 1.5f;
		[SF] private float staringReleaseSeconds = 0.5f;
		[SF] private float meowLookExtensionSeconds = 1f;

		public float TurningSeconds => turningSeconds;
		public float AlertDelayMinimum => alertDelayMinimum;
		public float AlertDelayMaximum => alertDelayMaximum;
		public float StaringReleaseSeconds => staringReleaseSeconds;
		public float MeowLookExtensionSeconds => meowLookExtensionSeconds;
	}
}
