using System.Collections.Generic;
using System.Reflection;
using Code.Infrastructure.EntityComponentSystem.SceneEntities;
using UnityEditor;
using UnityEngine;

namespace Code.Editor.Guid
{
	public class SceneEntityPrefabGuidRegenerator : AssetModificationProcessor
	{
		private static readonly Queue<string> _pendingAssetPaths = new();
		private static readonly FieldInfo _guidField = typeof(SceneEntity)
			.GetField("sceneEntityGuid", BindingFlags.NonPublic | BindingFlags.Instance);
        
		private static bool _isDelayCallScheduled;

		private static void OnWillCreateAsset(string assetPath)
		{
			if (assetPath.EndsWith(".prefab") == false)
				return;

			_pendingAssetPaths.Enqueue(assetPath);

			if (_isDelayCallScheduled)
				return;

			_isDelayCallScheduled = true;
			EditorApplication.delayCall += OnDelayedGuidRegeneration;
		}

		private static void OnDelayedGuidRegeneration()
		{
			EditorApplication.delayCall -= OnDelayedGuidRegeneration;
			_isDelayCallScheduled = false;

			while (_pendingAssetPaths.Count > 0)
			{
				string assetPath = _pendingAssetPaths.Dequeue();
				RegenerateGuidOnPrefab(assetPath);
			}
		}

		private static void RegenerateGuidOnPrefab(string assetPath)
		{
			if (_guidField == null) return;

			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
			if (prefab == null) return;

			SceneEntity entity = prefab.GetComponentInChildren<SceneEntity>(true);
			if (entity == null) return;

			_guidField.SetValue(entity, System.Guid.NewGuid().ToString());

			EditorUtility.SetDirty(prefab);
			AssetDatabase.SaveAssetIfDirty(prefab);
		}
	}
}