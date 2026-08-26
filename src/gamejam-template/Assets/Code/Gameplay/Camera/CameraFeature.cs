using Code.Gameplay.Camera.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Camera
{
	public sealed class CameraFeature : Feature
	{
		public CameraFeature(ISystemFactory systems)
		{
			Add(systems.Create<InitiateCameraShakeByRequestSystem>());
		}
	}
}
