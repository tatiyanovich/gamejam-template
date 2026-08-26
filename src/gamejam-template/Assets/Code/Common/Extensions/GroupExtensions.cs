using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Common.Extensions
{
	public static class GroupExtensions
	{
		public static GameEntity FindNearest(
			this IGroup<GameEntity> group,
			Vector3 fromPosition,
			List<GameEntity> buffer)
		{
			GameEntity nearest = null;
			float nearestDistanceSqr = float.MaxValue;

			foreach (GameEntity entity in group.GetEntities(buffer))
			{
				float distanceSqr = (entity.WorldPosition - fromPosition).sqrMagnitude;

				if (distanceSqr < nearestDistanceSqr)
				{
					nearestDistanceSqr = distanceSqr;
					nearest = entity;
				}
			}

			return nearest;
		}
	}
}
