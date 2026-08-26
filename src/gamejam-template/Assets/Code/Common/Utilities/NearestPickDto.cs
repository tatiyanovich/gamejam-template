using UnityEngine;

namespace Code.Common.Utilities
{
	public readonly struct NearestPickDto
	{
		public readonly Vector3 Center;
		public readonly float Radius;

		// 0 means every candidate inside the radius.
		public readonly int Limit;

		public NearestPickDto(Vector3 center, float radius, int limit)
		{
			Center = center;
			Radius = radius;
			Limit = limit;
		}
	}
}
