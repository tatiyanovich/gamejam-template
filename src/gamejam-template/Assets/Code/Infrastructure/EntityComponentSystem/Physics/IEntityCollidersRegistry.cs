using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Physics
{
	public interface IEntityCollidersRegistry
	{
		void Register(int instanceId, IEntity entity);
		void Unregister(int instanceId);
		TEntity Get<TEntity>(int instanceId) where TEntity : class;
	}
}