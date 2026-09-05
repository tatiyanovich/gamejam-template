using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowStrokeAnswerInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _strokeQuestions;

		public ShowStrokeAnswerInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_strokeQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.AnswerNeighbourSide,
					GameMatcher.AnswerStrokes,
					GameMatcher.AnswerProgress));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity question in _strokeQuestions)
				{
					board.GreyboxBoard.SetStrokeRow(
						question.AnswerNeighbourSide,
						question.AnswerStrokes,
						question.AnswerProgress);
				}
			}
		}
	}
}
