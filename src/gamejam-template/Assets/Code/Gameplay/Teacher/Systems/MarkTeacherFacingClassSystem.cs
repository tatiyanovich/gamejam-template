using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class MarkTeacherFacingClassSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _teachers;

		private readonly List<GameEntity> _buffer = new(1);

		public MarkTeacherFacingClassSystem(GameContext game)
		{
			_teachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention));
		}

		public void Execute()
		{
			foreach (GameEntity teacher in _teachers.GetEntities(_buffer))
			{
				teacher.isTeacherFacingClass = teacher.TeacherAttention.IsFacingClass();
			}
		}
	}
}
