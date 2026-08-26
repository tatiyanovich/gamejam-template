#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Code.Common.Utilities
{
	public static class GizmosUtility
	{
		public static void DrawPlumbob(
			Vector3 position,
			Color faceColor,
			Color edgeColor,
			float heightAbove = 2.5f,
			float halfHeight = 0.4f,
			float halfWidth = 0.25f,
			float bobAmplitude = 0.2f,
			float bobSpeed = 2f,
			float spinSpeed = 60f)
		{
			float bobOffset = Mathf.Sin((float)EditorApplication.timeSinceStartup * bobSpeed) * bobAmplitude;
			float spinAngle = (float)EditorApplication.timeSinceStartup * spinSpeed;

			Vector3 center = position + Vector3.up * (heightAbove + bobOffset);

			Vector3 top = center + Vector3.up * halfHeight;
			Vector3 bottom = center - Vector3.up * halfHeight;

			Quaternion rotation = Quaternion.Euler(0f, spinAngle, 0f);
			Vector3 right = rotation * Vector3.right * halfWidth;
			Vector3 forward = rotation * Vector3.forward * halfWidth;

			Vector3 midRight = center + right;
			Vector3 midLeft = center - right;
			Vector3 midForward = center + forward;
			Vector3 midBack = center - forward;

			Handles.color = faceColor;
			Handles.DrawAAConvexPolygon(top, midRight, midForward);
			Handles.DrawAAConvexPolygon(top, midForward, midLeft);
			Handles.DrawAAConvexPolygon(top, midLeft, midBack);
			Handles.DrawAAConvexPolygon(top, midBack, midRight);

			Handles.DrawAAConvexPolygon(bottom, midForward, midRight);
			Handles.DrawAAConvexPolygon(bottom, midLeft, midForward);
			Handles.DrawAAConvexPolygon(bottom, midBack, midLeft);
			Handles.DrawAAConvexPolygon(bottom, midRight, midBack);

			Handles.color = edgeColor;
			Handles.DrawLine(top, midRight);
			Handles.DrawLine(top, midLeft);
			Handles.DrawLine(top, midForward);
			Handles.DrawLine(top, midBack);

			Handles.DrawLine(bottom, midRight);
			Handles.DrawLine(bottom, midLeft);
			Handles.DrawLine(bottom, midForward);
			Handles.DrawLine(bottom, midBack);

			Handles.DrawLine(midRight, midForward);
			Handles.DrawLine(midForward, midLeft);
			Handles.DrawLine(midLeft, midBack);
			Handles.DrawLine(midBack, midRight);

			SceneView.RepaintAll();
		}
	}
}
#endif
