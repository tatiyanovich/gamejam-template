using System.Collections.Generic;
using Code.Gameplay.Exam;
using Entitas;
using Framework.Essentials.TimeManagement;

namespace Code.Gameplay.Teacher.Systems
{
	public class TickTeacherAttentionSystem : IExecuteSystem
	{
		private readonly ITimeService _timeService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _scheduledTeachers;

		private readonly List<GameEntity> _buffer = new(1);

		public TickTeacherAttentionSystem(GameContext game, ITimeService timeService)
		{
			_timeService = timeService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_scheduledTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttentionTimeLeft));
		}

		public void Execute()
		{
			if (_runningExams.count == 0)
				return;

			foreach (GameEntity teacher in _scheduledTeachers.GetEntities(_buffer))
			{
				float timeLeft = teacher.TeacherAttentionTimeLeft - _timeService.DeltaTime;

				teacher.ReplaceTeacherAttentionTimeLeft(timeLeft > 0 ? timeLeft : 0f);
			}
		}
	}
}
