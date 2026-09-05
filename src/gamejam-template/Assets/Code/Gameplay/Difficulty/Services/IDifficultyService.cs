using Code.Gameplay.Difficulty.Data;

namespace Code.Gameplay.Difficulty.Services
{
	public interface IDifficultyService
	{
		DifficultyPhase GetPhase(int questionIndex);
	}
}
