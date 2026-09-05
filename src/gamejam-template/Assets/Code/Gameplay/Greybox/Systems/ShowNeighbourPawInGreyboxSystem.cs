using Code.Gameplay.Difficulty.Services;
using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowNeighbourPawInGreyboxSystem : IExecuteSystem
	{
		private readonly IDifficultyService _difficultyService;

		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _neighbours;

		public ShowNeighbourPawInGreyboxSystem(GameContext game, IDifficultyService difficultyService)
		{
			_difficultyService = difficultyService;

			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex));

			_neighbours = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Neighbour,
					GameMatcher.NeighbourSide,
					GameMatcher.PawWindowTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity run in _runs)
				{
					ShowPaws(board, _difficultyService.GetPhase(run.CurrentQuestionIndex).PawWindow);
				}
			}
		}

		private void ShowPaws(GameEntity board, float pawWindow)
		{
			foreach (GameEntity neighbour in _neighbours)
			{
				board.GreyboxBoard.SetPaw(
					neighbour.NeighbourSide,
					neighbour.isPawLifted,
					neighbour.PawWindowTimeLeft / pawWindow);
			}
		}
	}
}
