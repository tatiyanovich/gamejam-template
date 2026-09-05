namespace Code.Gameplay.Exam.Services
{
	public interface IExamGradeService
	{
		ExamGrade GetGrade(int answersCopied);
		int GetStars(int answersCopied, int ducksThrown, int almostCaughtCount);
	}
}
