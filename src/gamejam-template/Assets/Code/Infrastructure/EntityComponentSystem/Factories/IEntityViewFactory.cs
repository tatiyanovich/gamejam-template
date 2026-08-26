using Cysharp.Threading.Tasks;

namespace Code.Infrastructure.EntityComponentSystem.Factories
{
	public interface IEntityViewFactory
	{
		string GetViewPath(GameEntity entity);
		EntityView CreateViewFromResourcesPath(GameEntity entity);
		EntityView CreateViewFromPrefab(GameEntity entity);
		UniTask<EntityView> CreateViewFromAssetReference(GameEntity entity);
		UniTask<EntityView> CreateViewFromAddressableKey(GameEntity entity);
	}
}