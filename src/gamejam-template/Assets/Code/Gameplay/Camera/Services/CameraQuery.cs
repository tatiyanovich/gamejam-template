using Entitas;

namespace Code.Gameplay.Camera.Services
{
	public class CameraQuery : ICameraQuery
	{
		private readonly IGroup<GameEntity> _cameras;

		public CameraQuery(GameContext game)
		{
			_cameras = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Camera));
		}
		
		public UnityEngine.Camera GetCamera()
		{
			UnityEngine.Camera fallback = null;

			foreach (GameEntity cameraEntity in _cameras)
			{
				UnityEngine.Camera camera = cameraEntity.Camera;

				if (camera == null)
					continue;

				fallback ??= camera;

				if (camera.isActiveAndEnabled)
					return camera;
			}

			return UnityEngine.Camera.main != null
				? UnityEngine.Camera.main
				: fallback;
		}
	}
}
