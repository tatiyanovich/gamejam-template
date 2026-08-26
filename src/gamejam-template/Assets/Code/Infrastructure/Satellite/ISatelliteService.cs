using System;

namespace Code.Infrastructure.Satellite
{
	public interface ISatelliteService
	{
		event Action<bool> OnConnectionChanged;
		bool HasConnection();
	}
}
