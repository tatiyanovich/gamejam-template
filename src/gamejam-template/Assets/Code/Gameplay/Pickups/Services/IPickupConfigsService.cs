using Code.Gameplay.Pickups.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Pickups.Services
{
	public interface IPickupConfigsService : IConfigsService
	{
		PickupsConfig PickupsConfig { get; }
	}
}
