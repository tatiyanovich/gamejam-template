namespace Code.Infrastructure.UiManagement.Services
{
	public class FallbackBaseCameraService : IFallbackBaseCameraService
	{
		private FallbackBaseCamera _fallbackBaseCamera;

		public void Register(FallbackBaseCamera fallbackBaseCamera)
		{
			_fallbackBaseCamera = fallbackBaseCamera;
		}

		public void EnableCamera()
		{
			_fallbackBaseCamera.EnableCamera();
		}

		public void DisableCamera()
		{
			_fallbackBaseCamera.DisableCamera();
		}
	}
}
