using Entitas;

namespace Code.Gameplay.Drilling.Systems
{
	public class TrackBestDrilledDistanceSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runs;

		public TrackBestDrilledDistanceSystem(GameContext game)
		{
			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun,
					GameMatcher.DrilledDistance,
					GameMatcher.BestDrilledDistance));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runs)
			{
				if (run.DrilledDistance <= run.BestDrilledDistance)
					continue;

				run.ReplaceBestDrilledDistance(run.DrilledDistance);
			}
		}
	}
}
