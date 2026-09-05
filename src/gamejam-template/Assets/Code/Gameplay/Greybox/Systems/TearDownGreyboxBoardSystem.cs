using Entitas;
using UnityEngine;

namespace Code.Gameplay.Greybox.Systems
{
	public class TearDownGreyboxBoardSystem : ITearDownSystem
	{
		private readonly IGroup<GameEntity> _boards;

		public TearDownGreyboxBoardSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));
		}

		public void TearDown()
		{
			foreach (GameEntity board in _boards)
			{
				Object.Destroy(board.GreyboxBoard.gameObject);
			}
		}
	}
}
