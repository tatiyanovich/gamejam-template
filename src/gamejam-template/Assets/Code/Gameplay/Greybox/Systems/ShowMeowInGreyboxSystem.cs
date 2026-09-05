using Code.Gameplay.Meow.Services;
using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowMeowInGreyboxSystem : IExecuteSystem
	{
		private readonly IMeowConfigsService _meowConfigsService;

		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _meowSources;

		public ShowMeowInGreyboxSystem(GameContext game, IMeowConfigsService meowConfigsService)
		{
			_meowConfigsService = meowConfigsService;

			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_meowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource,
					GameMatcher.MicrophoneLevel));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity meowSource in _meowSources)
				{
					board.GreyboxBoard.SetMeow(
						meowSource.MicrophoneLevel,
						_meowConfigsService.MeowConfig.ThresholdLevel,
						meowSource.isMeowArmed && meowSource.isOnCooldown == false);
				}
			}
		}
	}
}
