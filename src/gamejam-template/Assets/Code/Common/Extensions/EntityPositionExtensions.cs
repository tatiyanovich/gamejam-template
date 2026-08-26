using UnityEngine;

namespace Code.Common.Extensions
{
	public static class EntityPositionExtensions
	{
		public static float GetPlanarDistanceTo(this GameEntity entity, Vector3 targetPosition)
		{
			Vector2 entityPosition = new(entity.WorldPosition.x, entity.WorldPosition.y);
			Vector2 target = new(targetPosition.x, targetPosition.y);

			return Vector2.Distance(entityPosition, target);
		}
	}
}
