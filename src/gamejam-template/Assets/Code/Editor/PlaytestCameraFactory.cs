using Code.Gameplay.Camera;
using Code.Gameplay.Camera.Services;
using UnityEngine;

namespace Code.Editor
{
	public class PlaytestCameraFactory : ICameraFactory
	{
		public int ShakeCount { get; private set; }
		public CameraShakeTypeId LastShakeType { get; private set; }

		public GameEntity CreateCamera(int playerId, Vector3 position) => null;

		public void CreateShakeRequest(CameraShakeTypeId shakeTypeId, float scale = 1f)
		{
			ShakeCount++;
			LastShakeType = shakeTypeId;
		}

		public GameEntity CreateStaticCamera(Vector3 position) => null;
	}
}
