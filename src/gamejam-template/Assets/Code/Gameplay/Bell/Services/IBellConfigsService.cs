using Code.Gameplay.Bell.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Bell.Services
{
	public interface IBellConfigsService : IConfigsService
	{
		BellConfig BellConfig { get; }
	}
}
