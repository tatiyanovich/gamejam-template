using Code.Gameplay.Duck.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Duck.Services
{
	public interface IDuckConfigsService : IConfigsService
	{
		DuckConfig DuckConfig { get; }
	}
}
