using Code.Infrastructure.EntityComponentSystem;
using SF = UnityEngine.SerializeField;

namespace Code.Gameplay.Camera.Scene
{
	public class CameraView : EntityComponentProvider
	{
		[SF] private new UnityEngine.Camera camera;

		public override void RegisterComponents()
		{
			Entity
				.AddCamera(camera)
				.AddCameraView(this);
		}

		public override void UnregisterComponents()
		{
			Entity
				.SafeRemoveCamera()
				.SafeRemoveCameraView();
		}
	}
}
