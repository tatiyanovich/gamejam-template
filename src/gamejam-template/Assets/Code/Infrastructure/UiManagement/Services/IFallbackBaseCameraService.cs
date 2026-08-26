namespace Code.Infrastructure.UiManagement.Services
{
	public interface IFallbackBaseCameraService
	{
		void Register(FallbackBaseCamera fallbackBaseCamera);
		void EnableCamera();
		void DisableCamera();
	}
}
