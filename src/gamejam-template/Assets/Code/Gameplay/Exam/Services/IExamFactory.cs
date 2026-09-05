namespace Code.Gameplay.Exam.Services
{
	public interface IExamFactory
	{
		GameEntity CreateRun();
		GameEntity CreateQuestion(int questionIndex);
	}
}
