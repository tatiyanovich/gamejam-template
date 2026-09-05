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

		public IReadOnlyList<QuestionDefinition> Questions => questions;
	}
}
