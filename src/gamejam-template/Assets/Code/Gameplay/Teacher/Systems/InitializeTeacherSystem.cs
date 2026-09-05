using Code.Gameplay.Teacher.Services;
using Entitas;

namespace Code.Gameplay.Teacher.Systems
{
	public class InitializeTeacherSystem : IInitializeSystem
	{
		private readonly ITeacherFactory _teacherFactory;

		private readonly IGroup<GameEntity> _teachers;

		public InitializeTeacherSystem(GameContext game, ITeacherFactory teacherFactory)
		{
			_teacherFactory = teacherFactory;

			_teachers = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Teacher));
		}

		public void Initialize()
		{
			if (_teachers.count > 0)
				return;

			_teacherFactory.CreateTeacher();
		}
	}
}
