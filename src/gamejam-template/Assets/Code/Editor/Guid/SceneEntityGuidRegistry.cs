using System.Reflection;
using Code.Infrastructure.EntityComponentSystem.SceneEntities;
using UnityEditor;
using UnityEngine;

namespace Code.Editor.Guid
{
	[InitializeOnLoad]
	public static class SceneEntityGuidRegistry
	{
		private static readonly FieldInfo _guidField = typeof(SceneEntity)
			.GetField("sceneEntityGuid", BindingFlags.NonPublic | BindingFlags.Instance);

		static SceneEntityGuidRegistry()
		{
			ObjectFactory.componentWasAdded += HandleComponentAdded;
			ObjectChangeEvents.changesPublished += HandleChangesPublished;
		}

		private static void HandleComponentAdded(Component component)
		{
			if (component is SceneEntity sceneEntity)
				AssignNewGuid(sceneEntity);
		}

		private static void HandleChangesPublished(ref ObjectChangeEventStream stream)
		{
			for (int i = 0; i < stream.length; i++)
			{
				if (stream.GetEventType(i) != ObjectChangeKind.CreateGameObjectHierarchy)
					continue;

				stream.GetCreateGameObjectHierarchyEvent(i, out CreateGameObjectHierarchyEventArgs args);
				GameObject root = EditorUtility.InstanceIDToObject(args.instanceId) as GameObject;

				if (root == null)
					continue;

				foreach (SceneEntity sceneEntity in root.GetComponentsInChildren<SceneEntity>(true))
					AssignNewGuid(sceneEntity);
			}
		}

		private static void AssignNewGuid(SceneEntity entity)
		{
			_guidField.SetValue(entity, System.Guid.NewGuid().ToString());
			EditorUtility.SetDirty(entity);
		}
	}
}
