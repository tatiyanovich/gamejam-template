using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowWordAnswerInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _wordQuestions;

		public ShowWordAnswerInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_wordQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.AnswerNeighbourSide,
					GameMatcher.AnswerWord,
					GameMatcher.AnswerProgress));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity question in _wordQuestions)
				{
					board.GreyboxBoard.SetWordRow(
						question.AnswerNeighbourSide,
						question.AnswerWord,
						question.AnswerProgress);
				}
			}
		}
	}
}
