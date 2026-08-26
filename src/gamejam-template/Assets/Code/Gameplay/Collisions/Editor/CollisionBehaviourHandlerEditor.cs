#if UNITY_EDITOR
using Code.Infrastructure.EntityComponentSystem;
using UnityEditor;
using UnityEngine;

namespace Code.Gameplay.Collisions.Editor
{
	[InitializeOnLoad]
	internal static class CollisionBehaviourHandlerEditor
	{
		static CollisionBehaviourHandlerEditor()
		{
			ObjectFactory.componentWasAdded += OnComponentWasAdded;
		}

		private static void OnComponentWasAdded(Component added)
		{
			if (added == null)
				return;

			PutCollisionBehaviourOnColliderAdded(added);
			PutCollisiionBehaviourOnEntityViewAdded(added);
			RecheckCollisionBehaviour(added);
		}

		private static void PutCollisionBehaviourOnColliderAdded(Component added)
		{
			bool isCollider = added is Collider or Collider2D;

			if (isCollider == false)
				return;

			GameObject go = added.gameObject;
			if (go == null)
				return;

			if (go.GetComponent<CollisionBehaviour>() == null)
			{
				EntityView entityView = go.GetComponentInParent<EntityView>(true);

				if (entityView != null)
				{
					CollisionBehaviour collisionBehaviour = Undo.AddComponent<CollisionBehaviour>(go);
					collisionBehaviour.SetEntityView(entityView);
				}
			}
		}
		
		private static void PutCollisiionBehaviourOnEntityViewAdded(Component added)
		{
			if (added is not EntityView entityView)
				return;

			GameObject go = added.gameObject;

			if (go == null)
				return;

			Collider[] colliders = go.GetComponentsInChildren<Collider>();

			foreach (Collider collider in colliders)
			{
				if (collider.gameObject.GetComponent<CollisionBehaviour>() == null)
				{
					CollisionBehaviour collisionBehaviour = Undo.AddComponent<CollisionBehaviour>(collider.gameObject);
					collisionBehaviour.SetEntityView(entityView);
				}
			}
		}

		private static void RecheckCollisionBehaviour(Component added)
		{
			GameObject go = added.gameObject;

			if (go == null)
				return;

			Collider[] colliders = go.GetComponentsInChildren<Collider>();

			foreach (Collider collider in colliders)
			{
				if (collider.gameObject.TryGetComponent(out CollisionBehaviour collisionBehaviour))
				{
					EntityView entityView = go.GetComponentInParent<EntityView>(true);

					if (entityView != null)
					{
						collisionBehaviour.SetEntityView(entityView);
					}
				}
			}
		}
	}
}
#endif