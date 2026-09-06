using Code.Infrastructure.Audio.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Infrastructure.Audio.Services
{
	public interface IAudioConfigsService : IConfigsService
	{
		AudioConfig AudioConfig { get; }
	}
}
