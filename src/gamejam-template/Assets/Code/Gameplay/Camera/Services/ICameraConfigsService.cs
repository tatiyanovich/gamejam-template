using Code.Gameplay.Camera.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Camera.Services
{
	public interface ICameraConfigsService : IConfigsService
	{
		CameraConfig PlayerCameraConfig { get; }
		ShakeConfig GetShakeConfig(CameraShakeTypeId shakeTypeId);
	}
}
