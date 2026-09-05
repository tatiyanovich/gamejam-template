using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Exam.Systems
{
	public sealed class MarkTutorialDodgedTeacherSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _runs;
		private readonly IGroup<GameEntity> _facingTeachers;
		private readonly IGroup<InputEntity> _leaningInputs;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkTutorialDodgedTeacherSystem(GameContext game, InputContext input)
		{
			_runs = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ExamRun)
				.NoneOf(
					GameMatcher.TutorialDodgedTeacher));

			_facingTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherFacingClass));

			_leaningInputs = input.GetGroup(InputMatcher
				.AllOf(
					InputMatcher.Input,
					InputMatcher.LeanHeld));
		}

		public void Execute()
		{
			if (_facingTeachers.count == 0 || _leaningInputs.count > 0)
				return;

			foreach (GameEntity run in _runs.GetEntities(_buffer))
				run.isTutorialDodgedTeacher = true;
		}
	}
}
