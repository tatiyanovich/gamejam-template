using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Code.Common.Utilities
{
	public static class DebugDraw
	{
		private static readonly List<Vector3> Vertices = new(1024);
		private static readonly List<Color> Colors = new(1024);
		private static readonly List<int> Indices = new(1024);

		private static Mesh _mesh;
		private static Material _material;
		private static DebugDrawGizmos _host;
		private static int _gizmosFrame = int.MinValue;

		private const int CircleSegments = 24;
		private const int DrawLayer = 0;
		private const int GizmosFrameTolerance = 2;
		private const string ShaderName = "Hidden/GameTemplate/DebugLine2D";

		[Conditional("UNITY_EDITOR")]
		public static void DrawCircle(Vector3 center, float radius, Color color)
		{
			float step = Mathf.PI * 2f / CircleSegments;
			Vector3 previous = center + new Vector3(radius, 0f, 0f);

			for (int i = 1; i <= CircleSegments; i++)
			{
				float angle = step * i;

				Vector3 current = center + new Vector3(
					Mathf.Cos(angle) * radius,
					Mathf.Sin(angle) * radius,
					0f);

				AddSegment(previous, current, color);
				previous = current;
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void DrawSquare(Vector3 origin, float size, Color color)
		{
			Vector3 bottomLeft = new(origin.x, origin.y, 0f);
			Vector3 bottomRight = new(origin.x + size, origin.y, 0f);
			Vector3 topRight = new(origin.x + size, origin.y + size, 0f);
			Vector3 topLeft = new(origin.x, origin.y + size, 0f);

			AddSegment(bottomLeft, bottomRight, color);
			AddSegment(bottomRight, topRight, color);
			AddSegment(topRight, topLeft, color);
			AddSegment(topLeft, bottomLeft, color);
		}

		[Conditional("UNITY_EDITOR")]
		public static void Flush()
		{
			if (Vertices.Count == 0)
				return;

			if (GizmosVisible())
				DrawBufferedMesh();

			Vertices.Clear();
			Colors.Clear();
			Indices.Clear();
		}

		internal static void MarkGizmosVisible()
		{
			_gizmosFrame = Time.frameCount;
		}

		private static bool GizmosVisible()
		{
			return Time.frameCount - _gizmosFrame <= GizmosFrameTolerance;
		}

		private static void DrawBufferedMesh()
		{
			EnsureResources();

			_mesh.Clear();
			_mesh.SetVertices(Vertices);
			_mesh.SetColors(Colors);
			_mesh.SetIndices(Indices, MeshTopology.Lines, 0);

			Graphics.DrawMesh(_mesh, Matrix4x4.identity, _material, DrawLayer);
		}

		private static void AddSegment(Vector3 from, Vector3 to, Color color)
		{
			EnsureHost();

			Indices.Add(Vertices.Count);
			Indices.Add(Vertices.Count + 1);

			Vertices.Add(from);
			Vertices.Add(to);

			Colors.Add(color);
			Colors.Add(color);
		}

		private static void EnsureHost()
		{
			if (_host != null)
				return;

			GameObject host = new(nameof(DebugDraw))
			{
				hideFlags = HideFlags.DontSave,
			};

			_host = host.AddComponent<DebugDrawGizmos>();
		}

		private static void EnsureResources()
		{
			if (_mesh == null)
			{
				_mesh = new Mesh
				{
					name = nameof(DebugDraw),
					hideFlags = HideFlags.HideAndDontSave,
				};

				_mesh.MarkDynamic();
			}

			if (_material == null)
			{
				_material = new Material(Shader.Find(ShaderName))
				{
					hideFlags = HideFlags.HideAndDontSave,
				};
			}
		}
	}
}
