using System;
using Code.Infrastructure.EntityComponentSystem;
using Entitas;

namespace Code.Gameplay.Teacher.Queries
{
	public sealed class TeacherQuery : ITeacherQuery, IReactiveQuery
	{
		private readonly IGroup<GameEntity> _teachers;
		private readonly IGroup<GameEntity> _changedTeachers;

		public event Action<TeacherAttention> OnAttentionChanged;
		public event Action<int> OnAlmostCaughtCountChanged;

		public TeacherQuery(GameContext game)
		{
			_teachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.AlmostCaughtCount));

			_changedTeachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.AlmostCaughtCount)
				.AnyOf(
					GameMatcher.TeacherAttentionChanged,
					GameMatcher.AlmostCaughtCountChanged));
		}

		public void ReactToChanges()
		{
			foreach (GameEntity teacher in _changedTeachers)
			{
				if (teacher.isTeacherAttentionChanged)
					OnAttentionChanged?.Invoke(teacher.TeacherAttention);

				if (teacher.isAlmostCaughtCountChanged)
					OnAlmostCaughtCountChanged?.Invoke(teacher.AlmostCaughtCount);
			}
		}

		public TeacherAttention GetAttention()
		{
			foreach (GameEntity teacher in _teachers)
				return teacher.TeacherAttention;

			return TeacherAttention.Writing;
		}

		public bool IsFacingClass()
		{
			foreach (GameEntity teacher in _teachers)
				return teacher.isTeacherFacingClass;

			return false;
		}

		public int GetAlmostCaughtCount()
		{
			foreach (GameEntity teacher in _teachers)
				return teacher.AlmostCaughtCount;

			return 0;
		}
	}
}
