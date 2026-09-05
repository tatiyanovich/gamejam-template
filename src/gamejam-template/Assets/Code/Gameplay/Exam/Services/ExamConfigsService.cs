using Code.Gameplay.Difficulty.Configs;
using Code.Gameplay.Exam.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Exam.Services
{
	public class ExamConfigsService : IExamConfigsService
	{
		private readonly IAssetsService _assets;

		private const string ExamConfigKey = "exam_config";
		private const string DifficultyConfigKey = "difficulty_config";

		public ExamConfig ExamConfig { get; private set; }
		public DifficultyConfig DifficultyConfig { get; private set; }

		public ExamConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			ExamConfig = _assets.Load<ExamConfig>(ExamConfigKey);
			DifficultyConfig = _assets.Load<DifficultyConfig>(DifficultyConfigKey);
		}
	}
}
