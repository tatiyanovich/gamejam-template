using System.Collections.Generic;
using Code.Gameplay.Difficulty.Data;
using Code.Gameplay.Difficulty.Services;
using Code.Gameplay.Exam;
using Code.Gameplay.Input;
using Code.Infrastructure.Randomization;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class WatchClassSystem : IExecuteSystem
	{
		private readonly IDifficultyService _difficultyService;
		private readonly IRandomService _randomService;

		private readonly IGroup<GameEntity> _runningExams;
		private readonly IGroup<GameEntity> _turningTeachers;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _runBuffer = new(1);
		private readonly List<GameEntity> _teacherBuffer = new(1);

		public WatchClassSystem(
			GameContext game,
			InputContext input,
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

			_turningTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.TeacherAttentionTimeLeft,
					GameMatcher.AlmostCaughtCount));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			foreach (GameEntity run in _runningExams.GetEntities(_runBuffer))
			{
				StartWatching(_difficultyService.GetPhase(run.CurrentQuestionIndex));
			}
		}

		private void StartWatching(DifficultyPhase phase)
		{
			foreach (GameEntity teacher in _turningTeachers.GetEntities(_teacherBuffer))
			{
				if (IsTurningToClass(teacher.TeacherAttention) == false)
					continue;

				if (teacher.TeacherAttentionTimeLeft > 0)
					continue;

				teacher.SwitchAttention(
					TeacherAttention.Watching,
					_randomService.Range(phase.LookDurationMinimum, phase.LookDurationMaximum));

				if (_leaningInputs.count > 0)
					teacher.ReplaceAlmostCaughtCount(teacher.AlmostCaughtCount + 1);
			}
		}

		private static bool IsTurningToClass(TeacherAttention attention)
		{
			return attention == TeacherAttention.Turning || attention == TeacherAttention.Alerted;
		}
	}
}
