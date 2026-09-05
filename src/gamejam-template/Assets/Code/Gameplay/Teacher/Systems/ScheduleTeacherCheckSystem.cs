using System.Collections.Generic;
using Code.Gameplay.Difficulty.Data;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Exam;
using Code.Infrastructure.Randomization;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class ScheduleTeacherCheckSystem : IExecuteSystem
	{
		private readonly IDifficultyService _difficultyService;
		private readonly IRandomService _randomService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _idleTeachers;

		private readonly List<GameEntity> _runBuffer = new(1);
		private readonly List<GameEntity> _teacherBuffer = new(1);

		public ScheduleTeacherCheckSystem(
			GameContext game,
			IDifficultyService difficultyService,
			IRandomService randomService)
		{
			_difficultyService = difficultyService;
			_randomService = randomService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex)
				.NoneOf(
					GameMatcher.ExamFinished));

			_idleTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention)
				.NoneOf(
					GameMatcher.TeacherAttentionTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runningExams.GetEntities(_runBuffer))
			{
				DifficultyPhase phase = _difficultyService.GetPhase(run.CurrentQuestionIndex);

				if (phase.TeacherChecks == false)
					continue;

				ScheduleCheck(phase);
			}
		}

		private void ScheduleCheck(DifficultyPhase phase)
		{
			foreach (GameEntity teacher in _idleTeachers.GetEntities(_teacherBuffer))
			{
				if (teacher.TeacherAttention != TeacherAttention.Writing)
					continue;

				teacher.AddTeacherAttentionTimeLeft(
					_randomService.Range(phase.CheckDelayMinimum, phase.CheckDelayMaximum));
			}
		}
	}
}
