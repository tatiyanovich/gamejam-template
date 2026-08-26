using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Timers.Systems
{
	public class ResetTimersSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _timers;
		private readonly List<GameEntity> _buffer = new(64);

		public ResetTimersSystem(GameContext game)
		{
			_timers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Timer,
					GameMatcher.TimeLeft,
					GameMatcher.Interval)
				.NoneOf(
					GameMatcher.IntervalUp,
					GameMatcher.Ticking));
		}

		public void Execute()
		{
			foreach (GameEntity timer in _timers.GetEntities(_buffer))
			{
				timer.ReplaceTimeLeft(timer.Interval);
				timer.isTicking = true;
			}
		}
	}
}
