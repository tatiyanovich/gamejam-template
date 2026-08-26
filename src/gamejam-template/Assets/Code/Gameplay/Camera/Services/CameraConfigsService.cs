using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Camera.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Camera.Services
{
	public class CameraConfigsService : ICameraConfigsService
	{
		private readonly IAssetsService _assets;
		private Dictionary<CameraShakeTypeId, ShakeConfig> _shakeConfigsByTypeId = new();

		private const string PlayerCameraConfigKey = "player_camera_config";
		private const string CameraShakeConfigsLabel = "camera_shake_configs";

		public CameraConfig PlayerCameraConfig { get; private set; }

		public CameraConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			LoadPlayerCameraConfig();
			LoadCameraShakeConfigs();
		}

		public ShakeConfig GetShakeConfig(CameraShakeTypeId shakeTypeId)
		{
			_shakeConfigsByTypeId.TryGetValue(shakeTypeId, out ShakeConfig config);
			return config;
		}

		private void LoadPlayerCameraConfig()
		{
			PlayerCameraConfig = _assets.Load<CameraConfig>(PlayerCameraConfigKey);
		}

		private void LoadCameraShakeConfigs()
		{
			_shakeConfigsByTypeId = _assets.GetAssetsByLabel<ShakeConfig>(CameraShakeConfigsLabel)
				.ToDictionary(config => config.ShakeTypeId, config => config);
		}
	}
}
