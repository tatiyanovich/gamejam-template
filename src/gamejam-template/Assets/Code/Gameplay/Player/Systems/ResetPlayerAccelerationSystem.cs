using Code.Gameplay.Player.Services;
using Entitas;

namespace Code.Gameplay.Player.Systems
{
	public sealed class ResetPlayerAccelerationSystem : IExecuteSystem
	{
		private readonly IPlayerConfigsService _configsService;

		private readonly IGroup<GameEntity> _players;

		public ResetPlayerAccelerationSystem(
			GameContext game,
			IPlayerConfigsService configsService)
		{
			_configsService = configsService;

			_players = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.MovementAcceleration));
		}

		public void Execute()
		{
			float baseAcceleration = _configsService.PlayerConfig.MoveAcceleration;

			foreach (GameEntity player in _players)
			{
				player.ReplaceMovementAcceleration(baseAcceleration);
			}
		}
	}
}
