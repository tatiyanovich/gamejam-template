using System.Collections.Generic;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Input;
using Code.Gameplay.Teacher.Services;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class KeepStaringWhileLeaningSystem : IExecuteSystem
	{
		private readonly IDifficultyService _difficultyService;
		private readonly ITeacherConfigsService _teacherConfigsService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _scheduledTeachers;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _runBuffer = new(1);
		private readonly List<GameEntity> _teacherBuffer = new(1);

		public KeepStaringWhileLeaningSystem(
			GameContext game,
			InputContext input,
			IDifficultyService difficultyService,
			ITeacherConfigsService teacherConfigsService)
		{
			_difficultyService = difficultyService;
			_teacherConfigsService = teacherConfigsService;

			_runningExams = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun,
					GameMatcher.CurrentQuestionIndex)
				.NoneOf(
					GameMatcher.ExamFinished));

			_scheduledTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.TeacherAttentionTimeLeft));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			if (_leaningInputs.count == 0)
				return;

			foreach (GameEntity run in _runningExams.GetEntities(_runBuffer))
			{
				if (_difficultyService.GetPhase(run.CurrentQuestionIndex).StaringEnabled == false)
					continue;

				KeepStaring();
			}
		}

		private void KeepStaring()
		{
			float staringReleaseSeconds = _teacherConfigsService.TeacherConfig.StaringReleaseSeconds;

			foreach (GameEntity teacher in _scheduledTeachers.GetEntities(_teacherBuffer))
			{
				if (teacher.TeacherAttention == TeacherAttention.Staring)
				{
					if (teacher.TeacherAttentionTimeLeft < staringReleaseSeconds)
						teacher.ReplaceTeacherAttentionTimeLeft(staringReleaseSeconds);
					continue;
				}

				if (teacher.TeacherAttention != TeacherAttention.Watching)
					continue;

				if (teacher.TeacherAttentionTimeLeft > 0)
					continue;

				teacher.SwitchAttention(TeacherAttention.Staring, staringReleaseSeconds);
			}
		}
	}
}
