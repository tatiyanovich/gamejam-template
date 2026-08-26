using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem.Physics
{
	public interface IEntityPhysicsService
	{
		IEnumerable<GameEntity> RaycastAll(
			Vector3 worldPosition,
			Vector3 direction,
			int layerMask,
			float distance = Mathf.Infinity,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore);

		GameEntity Raycast(
			Vector3 worldPosition, 
			Vector3 direction, 
			float distance, 
			int layerMask, 
			out RaycastHit hit,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore);

		GameEntity LineCast(Vector3 start, Vector3 end, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.Ignore);

		IEnumerable<GameEntity> SphereOverlap(
			Vector3 position, 
			float radius, 
			int layerMask,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore);

		int SphereOverlapNonAlloc(
			Vector3 position, float radius, int layerMask, GameEntity[] hitBuffer,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore);

		void OverlapCollider(Collider2D collider, List<GameEntity> results, ContactFilter2D filter = default);

		TEntity OverlapPoint<TEntity>(Vector3 worldPosition, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
			where TEntity : class;

		int OverlapSphere(
			Vector3 worldPos, 
			float radius, 
			Collider[] hits, 
			int layerMask,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore);
	}
}