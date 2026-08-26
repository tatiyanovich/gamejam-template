using Code.Infrastructure.EntityComponentSystem;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Physics;
using UnityEngine;
using Zenject;

namespace Code.Gameplay.Collisions
{
	public class CollisionBehaviour : EntityDependant
	{
		private IEntityFactory _entityFactory;
		private IEntityCollidersRegistry _collidersRegistry;
		private int _colliderInstanceId;

		[Inject]
		private void Construct(IEntityFactory entityFactory, IEntityCollidersRegistry collidersRegistry)
		{
			_entityFactory = entityFactory;
			_collidersRegistry = collidersRegistry;

			Collider collider3D = GetComponent<Collider>();
			Collider2D collider2D = GetComponent<Collider2D>();

			_colliderInstanceId = collider3D != null
				? collider3D.GetInstanceID()
				: collider2D != null
					? collider2D.GetInstanceID()
					: 0;
		}

		private void CreateCollisionEntity(int otherColliderInstanceId, CollisionTypeId typeId)
		{
			GameEntity collidedEntity = _collidersRegistry.Get<GameEntity>(otherColliderInstanceId);
			GameEntity thisColliderEntity = _collidersRegistry.Get<GameEntity>(_colliderInstanceId);

			GameEntity collisionEntity = _entityFactory.Game();
			collisionEntity.isCollision = true;

			switch (typeId)
			{
				case CollisionTypeId.TriggerEnter: collisionEntity.isTriggerEnter = true; break;
				case CollisionTypeId.TriggerExit: collisionEntity.isTriggerExit = true; break;
				case CollisionTypeId.TriggerStay: collisionEntity.isTriggerStay = true; break;
				case CollisionTypeId.CollisionEnter: collisionEntity.isCollisionEnter = true; break;
				case CollisionTypeId.CollisionExit: collisionEntity.isCollisionExit = true; break;
				case CollisionTypeId.CollisionStay: collisionEntity.isCollisionStay = true; break;
			}

			if (collidedEntity != null)
			{
				collisionEntity.AddCollidedId(collidedEntity.Id);
			}

			if (thisColliderEntity != null)
			{
				collisionEntity.AddColliderOwnerId(thisColliderEntity.Id);
			}
		}

		#region 3D
		public void OnTriggerEnter(Collider otherCollider)
		{
			CreateCollisionEntity(otherCollider.GetInstanceID(), CollisionTypeId.TriggerEnter);
		}

		public void OnTriggerExit(Collider otherCollider)
		{
			CreateCollisionEntity(otherCollider.GetInstanceID(), CollisionTypeId.TriggerExit);
		}

		public void OnTriggerStay(Collider otherCollider)
		{
			CreateCollisionEntity(otherCollider.GetInstanceID(), CollisionTypeId.TriggerStay);
		}

		private void OnCollisionEnter(UnityEngine.Collision other)
		{
			CreateCollisionEntity(other.collider.GetInstanceID(), CollisionTypeId.CollisionEnter);
		}

		private void OnCollisionExit(UnityEngine.Collision other)
		{
			CreateCollisionEntity(other.collider.GetInstanceID(), CollisionTypeId.CollisionExit);
		}

		private void OnCollisionStay(UnityEngine.Collision other)
		{
			CreateCollisionEntity(other.collider.GetInstanceID(), CollisionTypeId.CollisionStay);
		}
		#endregion
		#region 2D
		public void OnTriggerEnter2D(Collider2D otherCollider)
		{
			CreateCollisionEntity(otherCollider.GetInstanceID(), CollisionTypeId.TriggerEnter);
		}

		public void OnTriggerExit2D(Collider2D otherCollider)
		{
			CreateCollisionEntity(otherCollider.GetInstanceID(), CollisionTypeId.TriggerExit);
		}

		public void OnTriggerStay2D(Collider2D otherCollider)
		{
			CreateCollisionEntity(otherCollider.GetInstanceID(), CollisionTypeId.TriggerStay);
		}

		private void OnCollisionEnter2D(Collision2D other)
		{
			CreateCollisionEntity(other.collider.GetInstanceID(), CollisionTypeId.CollisionEnter);
		}

		private void OnCollisionExit2D(Collision2D other)
		{
			CreateCollisionEntity(other.collider.GetInstanceID(), CollisionTypeId.CollisionExit);
		}

		private void OnCollisionStay2D(Collision2D other)
		{
			CreateCollisionEntity(other.collider.GetInstanceID(), CollisionTypeId.CollisionStay);
		}
		#endregion
	}
}
