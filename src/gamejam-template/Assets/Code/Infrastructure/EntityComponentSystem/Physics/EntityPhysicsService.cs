using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem.Physics
{
	/// <summary>
	/// Service for performing physics queries that return entities.
	/// Basically a wrapper over Unity Physics API that makes it easier to get entities from colliders.
	/// </summary>
	public class EntityPhysicsService : IEntityPhysicsService
	{
		private readonly RaycastHit[] _hits = new RaycastHit[128];
		private readonly Collider[] _overlapHits = new Collider[128];
		private readonly Collider2D[] _overlap2DHits = new Collider2D[128];

		private readonly IEntityCollidersRegistry _entityCollidersRegistry;

		public EntityPhysicsService(IEntityCollidersRegistry entityCollidersRegistry)
		{
			_entityCollidersRegistry = entityCollidersRegistry;
		}

		public IEnumerable<GameEntity> RaycastAll(
			Vector3 worldPosition,
			Vector3 direction,
			int layerMask,
			float distance = Mathf.Infinity,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
		{
			if (direction.sqrMagnitude < 1e-8f)
				yield break;

			int hitCount = UnityEngine.Physics.RaycastNonAlloc(worldPosition, direction.normalized, _hits, distance, layerMask, query);
			HashSet<GameEntity> seen = new();

			for (int i = 0; i < hitCount; i++)
			{
				RaycastHit hit = _hits[i];
				if (hit.collider == null) 
					continue;

				GameEntity entity = _entityCollidersRegistry.Get<GameEntity>(hit.collider.GetInstanceID());
				if (entity == null) 
					continue;

				if (seen.Add(entity))
					yield return entity;
			}
		}

		public GameEntity Raycast(
			Vector3 worldPosition, 
			Vector3 direction, 
			float distance, 
			int layerMask, 
			out RaycastHit hit,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
		{
			int hitCount = UnityEngine.Physics.RaycastNonAlloc(worldPosition, direction.normalized, _hits, distance, layerMask, query);
			DrawRay(worldPosition, direction, distance, Color.red);

			for (int i = 0; i < hitCount; i++)
			{
				hit = _hits[i];
				if (hit.collider == null) 
					continue;

				GameEntity entity = _entityCollidersRegistry.Get<GameEntity>(hit.collider.GetInstanceID());
				if (entity == null) 
					continue;

				return entity;
			}

			hit = default;
			return null;
		}

		public GameEntity LineCast(Vector3 start, Vector3 end, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
		{
			if (UnityEngine.Physics.Linecast(start, end, out var hit, layerMask, query))
			{
				Vector3 dir = end - start;
				DrawRay(start, dir, dir.magnitude, Color.yellow);

				if (hit.collider != null)
				{
					GameEntity entity = _entityCollidersRegistry.Get<GameEntity>(hit.collider.GetInstanceID());
					if (entity != null)
						return entity;
				}
			}

			return null;
		}

		public IEnumerable<GameEntity> SphereOverlap(
			Vector3 position, 
			float radius, 
			int layerMask,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
		{
			int hitCount = OverlapSphere(position, radius, _overlapHits, layerMask, query);

			DrawDebugSphereAxes(position, radius, 1f, Color.red);

			for (int i = 0; i < hitCount; i++)
			{
				GameEntity entity = _entityCollidersRegistry.Get<GameEntity>(_overlapHits[i].GetInstanceID());
				if (entity == null) 
					continue;

				yield return entity;
			}
		}

		public int SphereOverlapNonAlloc(
			Vector3 position, float radius, int layerMask, GameEntity[] hitBuffer,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
		{
			int found = OverlapSphere(position, radius, _overlapHits, layerMask, query);
			DrawDebugSphereAxes(position, radius, 1f, Color.green);

			int write = 0;
			for (int i = 0; i < found && write < hitBuffer.Length; i++)
			{
				GameEntity entity = _entityCollidersRegistry.Get<GameEntity>(_overlapHits[i].GetInstanceID());
				
				if (entity == null) 
					continue;
				
				hitBuffer[write++] = entity;
			}
			
			return write;
		}

		public void OverlapCollider(Collider2D collider, List<GameEntity> results, ContactFilter2D filter = default)
		{
			results.Clear();

			int found = collider.Overlap(filter, _overlap2DHits);

			for (int i = 0; i < found; i++)
			{
				GameEntity entity = _entityCollidersRegistry.Get<GameEntity>(_overlap2DHits[i].GetInstanceID());

				if (entity == null)
					continue;

				results.Add(entity);
			}
		}

		public TEntity OverlapPoint<TEntity>(Vector3 worldPosition, int layerMask, QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
			where TEntity : class
		{
			// 3D physics doesn't have OverlapPoint; approximate with a tiny sphere
			const float KPointEpsilon = 0.01f;
			int hitCount = UnityEngine.Physics.OverlapSphereNonAlloc(worldPosition, KPointEpsilon, _overlapHits, layerMask, query);

			for (int i = 0; i < hitCount; i++)
			{
				Collider col = _overlapHits[i];
				
				if (col == null) 
					continue;

				TEntity entity = _entityCollidersRegistry.Get<TEntity>(col.GetInstanceID());
				
				if (entity == null) 
					continue;

				return entity;
			}

			return null;
		}

		public int OverlapSphere(
			Vector3 worldPos, 
			float radius, 
			Collider[] hits, 
			int layerMask,
			QueryTriggerInteraction query = QueryTriggerInteraction.Ignore)
		{
			int count = UnityEngine.Physics.OverlapSphereNonAlloc(worldPos, radius, hits, layerMask, query);

			Array.Sort(hits, 0, count, Comparer<Collider>.Create((a, b) =>
			{
				float da = (a.ClosestPoint(worldPos) - worldPos).sqrMagnitude;
				float db = (b.ClosestPoint(worldPos) - worldPos).sqrMagnitude;
				return da.CompareTo(db);
			}));

			return count;
		}

		private void DrawRay(Vector3 worldPos, Vector3 direction, float distance, Color color)
		{
#if UNITY_EDITOR
			Debug.DrawRay(worldPos, direction.normalized * distance, color, 1f);
#endif
		}

		private void DrawDebugSphereAxes(Vector3 worldPos, float radius, float seconds, Color color)
		{
#if UNITY_EDITOR
			// Debug.DrawRay(worldPos, radius * Vector3.up, color, seconds);
			// Debug.DrawRay(worldPos, radius * Vector3.down, color, seconds);
			// Debug.DrawRay(worldPos, radius * Vector3.left, color, seconds);
			// Debug.DrawRay(worldPos, radius * Vector3.right, color, seconds);
			// Debug.DrawRay(worldPos, radius * Vector3.forward, color, seconds);
			// Debug.DrawRay(worldPos, radius * Vector3.back, color, seconds);
#endif
		}
	}
}