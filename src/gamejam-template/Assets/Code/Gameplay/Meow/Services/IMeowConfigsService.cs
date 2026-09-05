using Code.Gameplay.Meow.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Meow.Services
{
	public interface IMeowConfigsService : IConfigsService
	{
		MeowConfig MeowConfig { get; }
	}
}
