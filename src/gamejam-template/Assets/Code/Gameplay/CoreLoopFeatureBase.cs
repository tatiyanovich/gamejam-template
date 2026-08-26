using Code.Gameplay.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.SceneEntities.Systems;

namespace Code.Gameplay
{
    public abstract class CoreLoopFeatureBase : Feature
    {
        protected CoreLoopFeatureBase(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<GlobalLoopInfraHeadFeature>());
            Add(systemFactory.Create<InitializeSceneEntitiesByRequestSystem>());

            AddOverloadSystems(systemFactory);

            Add(systemFactory.Create<GlobalLoopInfraTailFeature>());
        }

        protected abstract void AddOverloadSystems(ISystemFactory systemFactory);
    }
}
