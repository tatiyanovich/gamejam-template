using System;
using Code.Infrastructure.Satellite;

namespace Code.Editor
{
	public class PlaytestConnection : ISatelliteService
	{
		public bool Connected { get; set; } = true;

		public event Action<bool> OnConnectionChanged { add { } remove { } }

		public bool HasConnection() => Connected;
	}
}
