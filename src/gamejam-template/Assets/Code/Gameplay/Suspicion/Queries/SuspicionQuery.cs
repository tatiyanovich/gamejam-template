using System;
using Code.Gameplay.Suspicion.Services;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Suspicion.Queries
{
	public sealed class SuspicionQuery : ISuspicionQuery, IReactiveQuery
	{
		private readonly ISuspicionConfigsService _suspicionConfigsService;

		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _changedRuns;

		public event Action<float> OnLevelChanged;

		public SuspicionQuery(GameContext game, ISuspicionConfigsService suspicionConfigsService)
		{
			_suspicionConfigsService = suspicionConfigsService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel));

			_changedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.SuspicionLevel,
					GameMatcher.SuspicionLevelChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity run in _changedRuns)
				OnLevelChanged?.Invoke(run.SuspicionLevel);
		}

		public float GetLevel()
		{
			foreach (GameEntity run in _runs)
				return run.SuspicionLevel;

			return 0f;
		}

		public float GetMaximumLevel() => _suspicionConfigsService.SuspicionConfig.MaximumLevel;
	}
}
