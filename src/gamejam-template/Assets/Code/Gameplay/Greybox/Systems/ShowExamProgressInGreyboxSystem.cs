using Code.Gameplay.Exam.Services;
using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowExamProgressInGreyboxSystem : IExecuteSystem
	{
		private readonly IExamConfigsService _examConfigsService;

		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _runs;

		public ShowExamProgressInGreyboxSystem(GameContext game, IExamConfigsService examConfigsService)
		{
			_examConfigsService = examConfigsService;

			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.AnswersCopied,
					GameMatcher.ExamElapsedSeconds));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity run in _runs)
				{
					board.GreyboxBoard.SetProgress(
						run.AnswersCopied,
						_examConfigsService.ExamConfig.Questions.Count,
						run.ExamElapsedSeconds);
				}
			}
		}
	}
}
