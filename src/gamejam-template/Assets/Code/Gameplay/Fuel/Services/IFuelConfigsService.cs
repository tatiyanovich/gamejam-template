using Code.Gameplay.Fuel.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Fuel.Services
{
	public interface IFuelConfigsService : IConfigsService
	{
		FuelConfig FuelConfig { get; }
	}
}
