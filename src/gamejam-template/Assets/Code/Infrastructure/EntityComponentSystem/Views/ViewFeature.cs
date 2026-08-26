using Code.Gameplay.Lifetime.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Views.Systems;

namespace Code.Infrastructure.EntityComponentSystem.Views
{
    public sealed class ViewFeature : Feature
    {
        public ViewFeature(ISystemFactory systemFactory)
        {
            Add(systemFactory.Create<BindEntityViewFromPathSystem>());
            Add(systemFactory.Create<BindEntityViewFromPrefabSystem>());
            Add(systemFactory.Create<BindEntityViewFromAssetReferenceSystem>());
            Add(systemFactory.Create<BindEntityViewFromAddressableKeySystem>());
        }
    }
}