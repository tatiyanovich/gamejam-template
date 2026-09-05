using System.Collections.Generic;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Teacher.Configs;
using Code.Gameplay.Teacher.Services;
using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Code.Infrastructure.Randomization;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class AlertTeacherOnPencilSnapSystem : IExecuteSystem
	{
		private readonly IDifficultyService _difficultyService;
		private readonly ITeacherConfigsService _teacherConfigsService;
		private readonly IRandomService _randomService;

		private readonly IGroup<GameEntity> _wrongInputEvents;
		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _teachers;

		private readonly List<GameEntity> _runBuffer = new(1);
		private readonly List<GameEntity> _teacherBuffer = new(1);

		public AlertTeacherOnPencilSnapSystem(
			GameContext game,
			IDifficultyService difficultyService,
			ITeacherConfigsService teacherConfigsService,
			IRandomService randomService)
		{
			_difficultyService = difficultyService;
			_teacherConfigsService = teacherConfigsService;
			_randomService = randomService;

			_wrongInputEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.WrongInputEvent));

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex)
				.NoneOf(
					GameMatcher.ExamFinished));

			_teachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention));
		}

		public void Execute()
		{
			if (_wrongInputEvents.count == 0)
				return;

			foreach (GameEntity run in _runningExams.GetEntities(_runBuffer))
			{
				if (_difficultyService.GetPhase(run.CurrentQuestionIndex).PencilSnapAlerts == false)
					continue;

				AlertTeacher();
			}
		}

		private void AlertTeacher()
		{
			TeacherConfig config = _teacherConfigsService.TeacherConfig;

			foreach (GameEntity teacher in _teachers.GetEntities(_teacherBuffer))
			{
				if (teacher.TeacherAttention != TeacherAttention.Writing)
					continue;

				teacher.SwitchAttention(
					TeacherAttention.Alerted,
					_randomService.Range(config.AlertDelayMinimum, config.AlertDelayMaximum));
			}
		}
	}
}
