using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Environment.Behaviours
{
	[RequireComponent(typeof(SpriteRenderer))]
	public sealed class GroundFollower : MonoBehaviour
	{
		[Tooltip("Tiled sprite renderer that represents the ground. Defaults to the one on this object.")]
		[SF] private SpriteRenderer ground;

		[Tooltip("Extra world-space margin added to each side of the covered area.")]
		[SF, Min(0f)] private float padding = 1f;

		private UnityEngine.Camera _camera;

		private const float Epsilon = 0.0001f;

		private void Reset()
		{
			ground = GetComponent<SpriteRenderer>();
		}

		private void LateUpdate()
		{
			Refresh();
		}

		private void Refresh()
		{
			UnityEngine.Camera followed = ResolveCamera();

			if (followed == null || ground.sprite == null)
				return;

			Vector2 tile = ground.sprite.bounds.size;

			if (tile.x < Epsilon || tile.y < Epsilon)
				return;

			SnapTo(followed, tile);
			Resize(followed, tile);
		}

		private void SnapTo(UnityEngine.Camera followed, Vector2 tile)
		{
			Vector3 cameraPosition = followed.transform.position;

			transform.position = new Vector3(
				Mathf.Round(cameraPosition.x / tile.x) * tile.x,
				Mathf.Round(cameraPosition.y / tile.y) * tile.y,
				transform.position.z);
		}

		private void Resize(UnityEngine.Camera followed, Vector2 tile)
		{
			float halfHeight = followed.orthographicSize;
			float halfWidth = halfHeight * followed.aspect;

			Vector2 size = new(
				Mathf.Ceil((halfWidth * 2f + padding) / tile.x) * tile.x + tile.x,
				Mathf.Ceil((halfHeight * 2f + padding) / tile.y) * tile.y + tile.y);

			if (Vector2.SqrMagnitude(ground.size - size) > Epsilon)
				ground.size = size;
		}

		private UnityEngine.Camera ResolveCamera()
		{
			if (_camera == null)
				_camera = UnityEngine.Camera.main;

			return _camera;
		}
	}
}
