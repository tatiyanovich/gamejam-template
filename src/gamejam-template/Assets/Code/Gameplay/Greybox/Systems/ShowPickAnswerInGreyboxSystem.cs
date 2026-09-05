using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowPickAnswerInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _pickQuestions;

		public ShowPickAnswerInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_pickQuestions = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Question,
					GameMatcher.AnswerNeighbourSide,
					GameMatcher.AnswerOptions,
					GameMatcher.CorrectOptionIndex));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity question in _pickQuestions)
				{
					board.GreyboxBoard.SetPickRow(
						question.AnswerNeighbourSide,
						question.AnswerOptions,
						question.CorrectOptionIndex);
				}
			}
		}
	}
}
