using UnityEngine;

namespace Code.Common.Extensions
{
	public static class TransformExtensions
	{
		public static void AttachToParent(this Transform transform, Transform parent, Vector3 localPosition)
		{
			transform.SetParent(parent, false);
			transform.localPosition = localPosition;
			transform.localRotation = Quaternion.identity;
		}
	}
}
