using System;

namespace Code.Gameplay.Teacher.Queries
{
	public interface ITeacherQuery
	{
		event Action<TeacherRemark> OnRemark;
		event Action<TeacherAttention> OnAttentionChanged;
		event Action<int> OnAlmostCaughtCountChanged;

		TeacherAttention GetAttention();
		bool IsFacingClass();
		int GetAlmostCaughtCount();
	}
}
