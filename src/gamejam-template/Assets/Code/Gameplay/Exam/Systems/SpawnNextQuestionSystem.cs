using Code.Gameplay.Exam.Services;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public class SpawnNextQuestionSystem : IExecuteSystem
	{
		private readonly IExamFactory _examFactory;
		private readonly IExamConfigsService _examConfigsService;

		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _questions;

		public SpawnNextQuestionSystem(
			GameContext game,
			IExamFactory examFactory,
			IExamConfigsService examConfigsService)
		{
			_examFactory = examFactory;
			_examConfigsService = examConfigsService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex)
				.NoneOf(
					GameMatcher.ExamFinished));

			_questions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question));
		}

		public void Execute()
		{
			if (_questions.count > 0)
				return;

			foreach (GameEntity run in _runs)
			{
				if (run.CurrentQuestionIndex >= _examConfigsService.ExamConfig.Questions.Count)
					continue;

				_examFactory.CreateQuestion(run.CurrentQuestionIndex);
			}
		}
	}
}
