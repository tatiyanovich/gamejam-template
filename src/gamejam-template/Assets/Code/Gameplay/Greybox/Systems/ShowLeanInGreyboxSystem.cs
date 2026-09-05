using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowLeanInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<InputEntity> _leaningInputs;

		public ShowLeanInGreyboxSystem(GameContext game, InputContext input)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				board.GreyboxBoard.SetLean(_leaningInputs.count > 0);
			}
		}
	}
}
