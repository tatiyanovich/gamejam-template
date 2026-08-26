using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Physics
{
	/// <summary>
	/// Registry that keeps track of entities by their collider instance IDs
	/// </summary>
	public class EntityCollidersRegistry : IEntityCollidersRegistry
	{
		private readonly Dictionary<int, IEntity> _entityByInstanceId = new();

		public void Register(int instanceId, IEntity entity) => _entityByInstanceId.TryAdd(instanceId, entity);
		public void Unregister(int instanceId) => _entityByInstanceId.Remove(instanceId);

		public TEntity Get<TEntity>(int instanceId) where TEntity : class =>
			_entityByInstanceId.TryGetValue(instanceId, out IEntity entity)
				? entity as TEntity
				: null;
	}
}