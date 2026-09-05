using System;
using Code.Gameplay.Bell.Services;
using Code.Infrastructure.EntityComponentSystem;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Bell.Queries
{
	public sealed class BellQuery : IBellQuery, IReactiveQuery
	{
		private readonly IBellConfigsService _bellConfigsService;

		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _changedRuns;
		private readonly IGroup<GameEntity> _announcementEvents;

		public event Action<float> OnTimeLeftChanged;
		public event Action OnAnnounced;

		public BellQuery(GameContext game, IBellConfigsService bellConfigsService)
		{
			_bellConfigsService = bellConfigsService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamElapsedSeconds));

			_changedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamElapsedSeconds,
					GameMatcher.ExamElapsedSecondsChanged));

			_announcementEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.BellAnnouncementEvent));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity run in _changedRuns)
				OnTimeLeftChanged?.Invoke(GetTimeLeft(run.ExamElapsedSeconds));

			foreach (GameEntity announcementEvent in _announcementEvents)
				OnAnnounced?.Invoke();
		}

		public float GetTimeLeft()
		{
			foreach (GameEntity run in _runs)
				return GetTimeLeft(run.ExamElapsedSeconds);

			return _bellConfigsService.BellConfig.ExamSeconds;
		}

		public bool IsAnnounced()
		{
			foreach (GameEntity run in _runs)
				return run.isBellAnnounced;

			return false;
		}

		private float GetTimeLeft(float elapsedSeconds)
		{
			return Mathf.Max(0f, _bellConfigsService.BellConfig.ExamSeconds - elapsedSeconds);
		}
	}
}
