using Code.Gameplay.Debugging.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Debugging
{
	public sealed class DebuggingFeature : Feature
	{
		public DebuggingFeature(ISystemFactory systems)
		{
			Add(systems.Create<TriggerGameplayDebugActionsSystem>());
		}
	}
}
