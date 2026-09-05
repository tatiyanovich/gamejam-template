using System.Collections.Generic;
using Code.Gameplay.Difficulty.Data;
using Code.Gameplay.Exam.Services;

namespace Code.Gameplay.Difficulty.Services
{
	public class DifficultyService : IDifficultyService
	{
		private readonly IExamConfigsService _examConfigsService;

		public DifficultyService(IExamConfigsService examConfigsService)
		{
			_examConfigsService = examConfigsService;
		}

		public DifficultyPhase GetPhase(int questionIndex)
		{
			IReadOnlyList<DifficultyPhase> phases = _examConfigsService.DifficultyConfig.Phases;
			int firstQuestionIndex = 0;

			foreach (DifficultyPhase phase in phases)
			{
				if (questionIndex < firstQuestionIndex + phase.QuestionCount)
					return phase;

				firstQuestionIndex += phase.QuestionCount;
			}

			return phases[phases.Count - 1];
		}
	}
}
