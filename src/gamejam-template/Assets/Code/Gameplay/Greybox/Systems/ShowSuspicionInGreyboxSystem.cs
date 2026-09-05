using Code.Gameplay.Suspicion.Services;
using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowSuspicionInGreyboxSystem : IExecuteSystem
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;

		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _runs;

		public ShowSuspicionInGreyboxSystem(GameContext game, ISuspicionConfigsService suspicionConfigsService)
		{
			_suspicionConfigsService = suspicionConfigsService;

			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity run in _runs)
				{
					board.GreyboxBoard.SetSuspicion(
						run.SuspicionLevel,
						_suspicionConfigsService.SuspicionConfig.MaximumLevel);
				}
			}
		}
	}
}
