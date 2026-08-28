using Entitas;

namespace Code.Gameplay.Player.Systems
{
	public sealed class SetPlayerLookDirectionSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _players;

		public SetPlayerLookDirectionSystem(GameContext game)
		{
			_players = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Player,
					GameMatcher.Moving,
					GameMatcher.MovementDirection,
					GameMatcher.LookDirection));
		}

		public void Execute()
		{
			foreach (GameEntity player in _players)
			{
				player.ReplaceLookDirection(player.MovementDirection);
			}
		}
	}
}
