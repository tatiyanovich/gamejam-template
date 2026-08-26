using UnityEngine;

namespace Code.Common.Utilities
{
	[AddComponentMenu("")]
	internal sealed class DebugDrawGizmos : MonoBehaviour
	{
		private void OnDrawGizmos()
		{
			Camera camera = Camera.current;

			if (camera == null)
				return;

			if (camera.cameraType != CameraType.Game)
				return;

			DebugDraw.MarkGizmosVisible();
		}
	}
}
