using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Progress.Systems
{
	public class RecordBestExamResultSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _finishedRuns;
		private readonly IGroup<GameEntity> _progresses;

		private readonly List<GameEntity> _runBuffer = new(1);
		private readonly List<GameEntity> _progressBuffer = new(1);

		public RecordBestExamResultSystem(GameContext game)
		{
			_finishedRuns = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamFinished,
					GameMatcher.AnswersCopied,
					GameMatcher.ExamElapsedSeconds)
				.NoneOf(
					GameMatcher.BestResultRecorded));

			_progresses = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamProgress,
					GameMatcher.BestAnswers,
					GameMatcher.BestTimeSeconds));
		}

		public void Execute()
		{
			foreach (GameEntity run in _finishedRuns.GetEntities(_runBuffer))
			{
				run.isBestResultRecorded = true;

				foreach (GameEntity progress in _progresses.GetEntities(_progressBuffer))
				{
					RecordIfBetter(progress, run);
				}
			}
		}

		private static void RecordIfBetter(GameEntity progress, GameEntity run)
		{
			if (IsBetterThanBest(progress, run) == false)
				return;

			progress.ReplaceBestAnswers(run.AnswersCopied);
			progress.ReplaceBestTimeSeconds(run.ExamElapsedSeconds);
		}

		private static bool IsBetterThanBest(GameEntity progress, GameEntity run)
		{
			if (run.AnswersCopied != progress.BestAnswers)
				return run.AnswersCopied > progress.BestAnswers;

			return progress.BestTimeSeconds <= 0f || run.ExamElapsedSeconds < progress.BestTimeSeconds;
		}
	}
}
