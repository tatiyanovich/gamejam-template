using Code.Gameplay.Teardown;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay
{
    // The Launch node runs alone as a single pipeline (shared infra head/tail via CoreLoopFeatureBase).
    // The launch scene itself is unloaded by RunSessionState once the first session scene has loaded —
    // not here — because the additive session scene is not ready yet when this feature tears down.
    public class LaunchCoreFeature : CoreLoopFeatureBase
    {
        public LaunchCoreFeature(ISystemFactory systemFactory) : base(systemFactory)
        {
        }

        protected override void AddOverloadSystems(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<LaunchTeardownSystem>());
        }
    }
}
