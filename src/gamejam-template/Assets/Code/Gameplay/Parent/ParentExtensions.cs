using UnityEngine;

namespace Code.Gameplay.Parent
{
	public static class ParentExtensions
	{
		public static GameEntity SetParent(this GameEntity entity, Transform transform)
		{
			entity.ReplaceParent(transform);
			entity.isParentAttached = false;
			
			return entity;
		}
		
		public static GameEntity SetParent(this GameEntity entity, int id)
		{
			entity.ReplaceParentId(id);
			entity.isParentAttached = false;
			
			return entity;
		}
	}
}