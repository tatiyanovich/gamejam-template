using Entitas;

namespace Code.Gameplay.Movement.Systems
{
	public sealed class ResetMovementSpeedMultiplierSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _movers;

		public ResetMovementSpeedMultiplierSystem(GameContext game)
		{
			_movers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MovementSpeedMultiplier));
		}

		public void Execute()
		{
			foreach (GameEntity mover in _movers)
			{
				mover.ReplaceMovementSpeedMultiplier(1f);
			}
		}
	}
}
