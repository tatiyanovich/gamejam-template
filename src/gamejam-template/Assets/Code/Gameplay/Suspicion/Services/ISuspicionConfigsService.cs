using Code.Gameplay.Suspicion.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Suspicion.Services
{
	public interface ISuspicionConfigsService : IConfigsService
	{
		SuspicionConfig SuspicionConfig { get; }
	}
}
