using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowQuestionInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _questions;

		public ShowQuestionInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_questions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.QuestionIndex,
					GameMatcher.QuestionText,
					GameMatcher.QuestionType,
					GameMatcher.AnswerNeighbourSide));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity question in _questions)
				{
					board.GreyboxBoard.SetQuestion(
						question.QuestionIndex,
						question.QuestionType,
						question.AnswerNeighbourSide);

					board.GreyboxBoard.SetQuestionText(question.QuestionText);
					board.GreyboxBoard.SetCopied(question.isAnswerCopied);
				}
			}
		}
	}
}
