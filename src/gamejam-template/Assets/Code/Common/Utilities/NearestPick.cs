using System.Collections.Generic;

namespace Code.Common.Utilities
{
	public static class NearestPick
	{
		public static void Take(IReadOnlyList<GameEntity> candidates, NearestPickDto pick, List<GameEntity> result)
		{
			result.Clear();

			float radiusSquared = pick.Radius * pick.Radius;
			int limit = pick.Limit > 0 ? pick.Limit : candidates.Count;

			for (int found = 0; found < limit; found++)
			{
				GameEntity nearest = null;
				float nearestDistanceSquared = radiusSquared;

				foreach (GameEntity candidate in candidates)
				{
					if (result.Contains(candidate))
						continue;

					float distanceSquared = (candidate.WorldPosition - pick.Center).sqrMagnitude;

					if (distanceSquared >= nearestDistanceSquared)
						continue;

					nearestDistanceSquared = distanceSquared;
					nearest = candidate;
				}

				if (nearest == null)
					return;

				result.Add(nearest);
			}
		}
	}
}
