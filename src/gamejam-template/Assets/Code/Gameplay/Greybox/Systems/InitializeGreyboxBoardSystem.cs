using Code.Gameplay.Greybox.Services;
using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class InitializeGreyboxBoardSystem : IInitializeSystem
	{
		private readonly IGreyboxBoardFactory _greyboxBoardFactory;

		private readonly IGroup<GameEntity> _boards;

		public InitializeGreyboxBoardSystem(GameContext game, IGreyboxBoardFactory greyboxBoardFactory)
		{
			_greyboxBoardFactory = greyboxBoardFactory;

			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));
		}

		public void Initialize()
		{
			if (_boards.count > 0)
				return;

			_greyboxBoardFactory.CreateBoard();
		}
	}
}
