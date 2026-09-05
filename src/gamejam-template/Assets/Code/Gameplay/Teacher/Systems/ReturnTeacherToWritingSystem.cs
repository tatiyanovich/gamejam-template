using System.Collections.Generic;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class ReturnTeacherToWritingSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _scheduledTeachers;

		private readonly List<GameEntity> _buffer = new(1);

		public ReturnTeacherToWritingSystem(GameContext game)
		{
			_scheduledTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.TeacherAttentionTimeLeft));
		}

		public void Execute()
		{
			foreach (GameEntity teacher in _scheduledTeachers.GetEntities(_buffer))
			{
				if (ReturnsToWriting(teacher.TeacherAttention) == false)
					continue;

				if (teacher.TeacherAttentionTimeLeft > 0)
					continue;

				teacher.ReturnToWriting();
			}
		}

		private static bool ReturnsToWriting(TeacherAttention attention)
		{
			return attention == TeacherAttention.Watching
				|| attention == TeacherAttention.Staring
				|| attention == TeacherAttention.Distracted;
		}
	}
}
