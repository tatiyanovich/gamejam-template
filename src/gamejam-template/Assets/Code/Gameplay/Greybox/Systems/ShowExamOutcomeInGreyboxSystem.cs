using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowExamOutcomeInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _finishedRuns;

		public ShowExamOutcomeInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_finishedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamFinished,
					GameMatcher.ExamOutcome));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity run in _finishedRuns)
				{
					board.GreyboxBoard.SetOutcome(run.ExamOutcome);
				}
			}
		}
	}
}
