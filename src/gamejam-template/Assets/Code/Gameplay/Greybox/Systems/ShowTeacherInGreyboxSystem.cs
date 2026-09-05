using Entitas;

namespace Code.Gameplay.Greybox.Systems
{
	public class ShowTeacherInGreyboxSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _boards;
		private readonly IGroup<GameEntity> _teachers;

		public ShowTeacherInGreyboxSystem(GameContext game)
		{
			_boards = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.GreyboxBoard));

			_teachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher,
					GameMatcher.TeacherAttention,
					GameMatcher.AlmostCaughtCount));
		}

		public void Execute()
		{
			foreach (GameEntity board in _boards)
			{
				foreach (GameEntity teacher in _teachers)
				{
					board.GreyboxBoard.SetTeacher(
						teacher.TeacherAttention,
						teacher.isTeacherFacingClass,
						teacher.AlmostCaughtCount);
				}
			}
		}
	}
}
