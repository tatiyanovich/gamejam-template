using Code.Infrastructure.Audio.Configs;
using Framework.AssetManagement;

namespace Code.Infrastructure.Audio.Services
{
	public class AudioConfigsService : IAudioConfigsService
	{
		private readonly IAssetsService _assets;

		private const string AudioConfigKey = "audio_config";

		public AudioConfig AudioConfig { get; private set; }

		public AudioConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			AudioConfig = _assets.Load<AudioConfig>(AudioConfigKey);
		}
	}
}
