using Code.Gameplay.Meow.Configs;
using Code.Gameplay.Meow.Services;
using Code.Infrastructure.Microphone;

namespace Code.Editor
{
	public class AttendancePlaytestMicrophone : IMicrophoneService, IMeowConfigsService
	{
		public bool IsAvailable { get; set; } = true;
		public float Level { get; set; }
		public MeowConfig MeowConfig { get; }

		public AttendancePlaytestMicrophone(MeowConfig config)
		{
			MeowConfig = config;
		}

		public float GetRootMeanSquare() => Level / MeowConfig.LevelScale;

		public void LoadConfigs() { }
	}
}
