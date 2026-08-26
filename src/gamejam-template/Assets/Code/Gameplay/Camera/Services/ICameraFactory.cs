using UnityEngine;

namespace Code.Gameplay.Camera.Services
{
	public interface ICameraFactory
	{
		GameEntity CreateCamera(int playerId, Vector3 position);
		void CreateShakeRequest(CameraShakeTypeId shakeTypeId, float scale = 1f);

		/// <summary>Spawns a non-following world camera (camera_prefab) at the given position.</summary>
		GameEntity CreateStaticCamera(Vector3 position);
	}
}
