using Entitas;

namespace Code.Gameplay.Player.Systems
{
	// The drill faces where it drives. Swap the source component for PointerWorldPosition
	// if the game needs aim-independent-of-movement steering instead.
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
