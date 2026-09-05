namespace Code.Gameplay.Exam.Services
{
	public class ExamGradeService : IExamGradeService
	{
		public ExamGrade GetGrade(int answersCopied)
		{
			return answersCopied switch
			{
				>= 12 => ExamGrade.APlus,
				11 => ExamGrade.A,
				>= 9 => ExamGrade.B,
				>= 6 => ExamGrade.C,
				>= 3 => ExamGrade.D,
				_ => ExamGrade.F
			};
		}

		public int GetStars(int answersCopied, int ducksThrown, int almostCaughtCount)
		{
			if (GetGrade(answersCopied) != ExamGrade.APlus)
				return 0;

			if (ducksThrown == 0 && almostCaughtCount <= 1)
				return 3;

			return almostCaughtCount <= 3 ? 2 : 1;
		}
	}
}
