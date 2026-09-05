using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Exam.Systems
{
	public class AccumulateExamTimeSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _runs;

		public AccumulateExamTimeSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.ExamElapsedSeconds)
				.NoneOf(
					GameMatcher.ExamFinished));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runs)
			{
				run.ReplaceExamElapsedSeconds(run.ExamElapsedSeconds + _timeService.DeltaTime);
			}
		}
	}
}
