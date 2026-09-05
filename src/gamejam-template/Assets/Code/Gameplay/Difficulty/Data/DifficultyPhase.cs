using System;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Difficulty.Data
{
	[Serializable]
	public class DifficultyPhase
	{
		[SF] private int questionCount;
		[SF] private bool teacherChecks;
		[SF] private float checkDelayMinimum;
		[SF] private float checkDelayMaximum;
		[SF] private float lookDurationMinimum;
		[SF] private float lookDurationMaximum;
		[SF] private float meowAlertChance;
		[SF] private bool pencilSnapAlerts;
		[SF] private bool staringEnabled;
		[SF] private float pawWindow;

		public int QuestionCount => questionCount;
		public bool TeacherChecks => teacherChecks;
		public float CheckDelayMinimum => checkDelayMinimum;
		public float CheckDelayMaximum => checkDelayMaximum;
		public float LookDurationMinimum => lookDurationMinimum;
		public float LookDurationMaximum => lookDurationMaximum;
		public float MeowAlertChance => meowAlertChance;
		public bool PencilSnapAlerts => pencilSnapAlerts;
		public bool StaringEnabled => staringEnabled;
		public float PawWindow => pawWindow;
	}
}
