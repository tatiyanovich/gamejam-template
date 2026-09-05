using System.Collections.Generic;
using Code.Gameplay.Exam;
using Code.Gameplay.Teacher.Services;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class TelegraphTeacherTurnSystem : IExecuteSystem
	{
		private readonly ITeacherConfigsService _teacherConfigsService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _scheduledTeachers;

		private readonly List<GameEntity> _buffer = new(1);

		public TelegraphTeacherTurnSystem(GameContext game, ITeacherConfigsService teacherConfigsService)
		{
			_teacherConfigsService = teacherConfigsService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.ExamFinished));

			_scheduledTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.TeacherAttentionTimeLeft));
		}

		public void Execute()
		{
			if (_runningExams.count == 0)
				return;

			foreach (GameEntity teacher in _scheduledTeachers.GetEntities(_buffer))
			{
				if (teacher.TeacherAttention != TeacherAttention.Writing)
					continue;

				if (teacher.TeacherAttentionTimeLeft > 0)
					continue;

				teacher.SwitchAttention(TeacherAttention.Turning, _teacherConfigsService.TeacherConfig.TurningSeconds);
			}
		}
	}
}
