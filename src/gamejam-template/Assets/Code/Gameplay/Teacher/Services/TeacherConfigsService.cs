using Code.Gameplay.Teacher.Configs;
using Framework.AssetManagement;

namespace Code.Gameplay.Teacher.Services
{
	public class TeacherConfigsService : ITeacherConfigsService
	{
		private readonly IAssetsService _assets;

		private const string TeacherConfigKey = "teacher_config";

		public TeacherConfig TeacherConfig { get; private set; }

		public TeacherConfigsService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void LoadConfigs()
		{
			TeacherConfig = _assets.Load<TeacherConfig>(TeacherConfigKey);
		}
	}
}
