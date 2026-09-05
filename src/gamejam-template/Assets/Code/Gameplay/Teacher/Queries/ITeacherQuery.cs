using System;

namespace Code.Gameplay.Teacher.Queries
{
	public interface ITeacherQuery
	{
		event Action<TeacherAttention> OnAttentionChanged;
		event Action<int> OnAlmostCaughtCountChanged;

		TeacherAttention GetAttention();
		bool IsFacingClass();
		int GetAlmostCaughtCount();
	}
}
