namespace Code.Gameplay.Exam
{
	public static class ExamGradeExtensions
	{
		public static string GetLabel(this ExamGrade grade)
		{
			return grade switch
			{
				ExamGrade.APlus => "A+",
				ExamGrade.A => "A",
				ExamGrade.B => "B",
				ExamGrade.C => "C",
				ExamGrade.D => "D",
				_ => "F"
			};
		}
	}
}
