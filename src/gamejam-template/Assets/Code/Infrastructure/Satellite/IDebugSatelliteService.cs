namespace Code.Infrastructure.Satellite
{
	public interface IDebugSatelliteService
	{
		bool IsForceOfflineEnabled();
		void SetForceOfflineEnabled(bool enabled);
	}
}
