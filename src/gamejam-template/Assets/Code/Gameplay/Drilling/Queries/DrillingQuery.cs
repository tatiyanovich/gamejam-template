using System;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Drilling.Queries
{
	public class DrillingQuery : IDrillingQuery, IReactiveQuery
	{
		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _changedRuns;
		private readonly IGroup<GameEntity> _finishedRuns;

		public event Action<float> OnDistanceChanged;
		public event Action OnRunFinished;

		public DrillingQuery(GameContext game)
		{
			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun,
					GameMatcher.DrilledDistance,
					GameMatcher.BestDrilledDistance));

			_changedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun,
					GameMatcher.DrilledDistance,
					GameMatcher.DrilledDistanceChanged));

			_finishedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.DrillRun,
					GameMatcher.RunFinished,
					GameMatcher.RunFinishedChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity run in _changedRuns)
			{
				OnDistanceChanged?.Invoke(run.DrilledDistance);
			}

			foreach (GameEntity _ in _finishedRuns)
				OnRunFinished?.Invoke();
		}

		public float GetDistance()
		{
			foreach (GameEntity run in _runs)
				return run.DrilledDistance;

			return 0f;
		}

		public float GetBestDistance()
		{
			foreach (GameEntity run in _runs)
				return run.BestDrilledDistance;

			return 0f;
		}
	}
}
