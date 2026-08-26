using Code.Infrastructure.CoreLoop;
using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem.Destruct
{
	public static class DestructExtensions
	{
		public static GameEntity InheritLoopNodeScope(this GameEntity entity, GameEntity owner)
		{
			if (owner.hasLoopNodeScope)
				entity.ReplaceLoopNodeScope(owner.loopNodeScope.NodeId);
			else
				entity.SafeRemoveLoopNodeScope();

			return entity;
		}

		public static void MarkNodeScopedEntitiesDestructed(this GameContext game)
		{
			foreach (GameEntity entity in game.GetEntities())
			{
				if (entity.isPersistAcrossLoopNodes)
				{
					WarnIfPersistentHoldsView(entity);
					continue;
				}

				entity.isDestructed = true;
			}
		}

		public static void MarkNodeScopedEntitiesDestructed(this GameContext game, LoopNodeId nodeId)
		{
			foreach (GameEntity entity in game.GetEntities())
			{
				if (entity.hasLoopNodeScope == false || entity.loopNodeScope.NodeId != nodeId)
					continue;

				if (entity.isPersistAcrossLoopNodes)
				{
					WarnIfPersistentHoldsView(entity);
					continue;
				}

				entity.isDestructed = true;
			}
		}

		private static void WarnIfPersistentHoldsView(GameEntity entity)
		{
			if (entity.hasView)
				Debug.LogError(
					$"Persistent entity (creationIndex {entity.creationIndex}) holds a View. " +
					"Views are destroyed on scene load; persistent entities must be view-less.");
		}
	}
}
