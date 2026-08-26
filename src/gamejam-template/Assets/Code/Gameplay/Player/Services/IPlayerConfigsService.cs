using Code.Gameplay.Player.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Player.Services
{
	public interface IPlayerConfigsService : IConfigsService
	{
		PlayerConfig PlayerConfig { get; }
	}
}
