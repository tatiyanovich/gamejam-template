namespace Code.Gameplay.Teacher
{
	public static class TeacherExtensions
	{
		public static void SwitchAttention(this GameEntity teacher, TeacherAttention attention, float timeLeft)
		{
			teacher.ReplaceTeacherAttention(attention);
			teacher.ReplaceTeacherAttentionTimeLeft(timeLeft);
		}

		public static void ReturnToWriting(this GameEntity teacher)
		{
			teacher.ReplaceTeacherAttention(TeacherAttention.Writing);
			teacher.RemoveTeacherAttentionTimeLeft();
		}

		public static bool IsFacingClass(this TeacherAttention attention)
		{
			return attention == TeacherAttention.Watching || attention == TeacherAttention.Staring;
		}
	}
}
