using Code.Gameplay.Difficulty.Configs;
using Code.Gameplay.Exam.Configs;
using Code.Infrastructure.ConfigsManagement;

namespace Code.Gameplay.Exam.Services
{
	public interface IExamConfigsService : IConfigsService
	{
		ExamConfig ExamConfig { get; }
		DifficultyConfig DifficultyConfig { get; }
	}
}
