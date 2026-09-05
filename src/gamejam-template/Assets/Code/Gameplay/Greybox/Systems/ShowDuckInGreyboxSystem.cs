using Code.Gameplay.Duck;
using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowDuckInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _settledDucks;
		private readonly IGroup<GameEntity> _timedDucks;

		public ShowDuckInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_settledDucks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckThrowCount)
				.NoneOf(
					GameMatcher.DuckStateTimeLeft));

			_timedDucks = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Duck,
					GameMatcher.DuckState,
					GameMatcher.DuckThrowCount,
					GameMatcher.DuckStateTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity duck in _settledDucks)
				{
					board.GreyboxBoard.SetDuck(duck.DuckState, 0f, duck.DuckThrowCount);
				}

				foreach (GameEntity duck in _timedDucks)
				{
					board.GreyboxBoard.SetDuck(duck.DuckState, duck.DuckStateTimeLeft, duck.DuckThrowCount);
				}
			}
		}
	}
}
