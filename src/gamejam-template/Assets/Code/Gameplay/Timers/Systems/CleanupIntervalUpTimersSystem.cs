using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Timers.Systems
{
	public class CleanupIntervalUpTimersSystem : ICleanupSystem
	{
		private readonly IGroup<GameEntity> _timers;
		private readonly List<GameEntity> _buffer = new(64);

		public CleanupIntervalUpTimersSystem(GameContext game)
		{
			_timers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Timer,
					GameMatcher.Interval,
					GameMatcher.TimeLeft,
					GameMatcher.IntervalUp));
		}

		public void Cleanup()
		{
			foreach (GameEntity timer in _timers.GetEntities(_buffer))
			{
				timer.isIntervalUp = false;
				timer.ReplaceTimeLeft(timer.Interval);
			}
		}
	}
}
