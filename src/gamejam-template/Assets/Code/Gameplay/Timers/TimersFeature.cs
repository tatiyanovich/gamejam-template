using Code.Gameplay.Timers.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Timers
{
	public sealed class TimersFeature : Feature
	{
		public TimersFeature(ISystemFactory systems)
		{
			Add(systems.Create<ResetTimersSystem>());
			Add(systems.Create<TimerTickSystem>());

			Add(systems.Create<CleanupIntervalUpTimersSystem>());
		}
	}
}