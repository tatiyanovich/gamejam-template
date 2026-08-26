using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Timers.Systems
{
	public class TimerTickSystem : IExecuteSystem
	{
		private readonly ITimeService _time;
		private readonly IGroup<GameEntity> _timers;

		public TimerTickSystem(GameContext game, ITimeService time)
		{
			_time = time;
			_timers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Timer,
					GameMatcher.TimeLeft,
					GameMatcher.Ticking));
		}

		public void Execute()
		{
			foreach (GameEntity timer in _timers)
			{
				timer.ReplaceTimeLeft(timer.TimeLeft - _time.DeltaTime);

				if (timer.TimeLeft <= 0)
					timer.isIntervalUp = true;
			}
		}
	}
}
