using Code.Common.Cooldown;
using Code.Gameplay.Camera;
using Code.Gameplay.Collisions;
using Code.Gameplay.Debugging;
using Code.Gameplay.Input;
using Code.Gameplay.Lifetime;
using Code.Gameplay.Parent;
using Code.Gameplay.Timers;
using Code.Gameplay.Vfx;
using Code.Infrastructure.EntityComponentSystem.Destruct;
using Code.Infrastructure.EntityComponentSystem.Events.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Storage.Systems;

namespace Code.Gameplay.CoreLoop
{
	// Shared ECS infrastructure that must run ONCE per frame, AFTER every branch's gameplay:
	// request handling, query-change notification, destruct processing and event cleanup all
	// operate on the single shared GameContext, so they must not be duplicated per branch.
	// Runs from SessionsRunningState (sessions) and from CoreLoopFeatureBase (the Launch node).
	public sealed class GlobalLoopInfraTailFeature : Feature
	{
		public GlobalLoopInfraTailFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<TimersFeature>());
			Add(systemFactory.Create<CooldownFeature>());
			Add(systemFactory.Create<AutoSaveSystem>(5));

			Add(systemFactory.Create<InputFeature>());
			Add(systemFactory.Create<DebuggingFeature>());

			Add(systemFactory.Create<LifetimeFeature>());

			Add(systemFactory.Create<VfxFeature>());

			Add(systemFactory.Create<CollisionsFeature>());

			Add(systemFactory.Create<ParentFeature>());

			Add(systemFactory.Create<CameraFeature>());

			Add(systemFactory.Create<SaveProgressByRequestSystem>());

			Add(systemFactory.Create<CoreLoopFeature>());

			Add(systemFactory.Create<OrphanRequestDetectionSystem>());
			Add(systemFactory.Create<NotifyQueryChangesSystem>());
			Add(systemFactory.Create<ProcessDestructedFeature>());
			Add(systemFactory.Create<GameWatchedCleanupSystems>());
			Add(systemFactory.Create<EventsCleanupSystem>());
		}
	}
}
