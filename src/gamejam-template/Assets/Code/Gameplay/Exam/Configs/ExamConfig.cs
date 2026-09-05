using System.Collections.Generic;
using Code.Gameplay.Exam.Data;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Exam.Configs
{
	[CreateAssetMenu(fileName = "ExamConfig", menuName = "Configs/Exam/ExamConfig")]
	public class ExamConfig : ScriptableObject
	{
		[SF] private List<QuestionDefinition> questions = new();
		[SF] private float questionPauseSeconds = 0.6f;

		public IReadOnlyList<QuestionDefinition> Questions => questions;
		public float QuestionPauseSeconds => questionPauseSeconds;
	}
}
