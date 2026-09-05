using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.Teacher.Services
{
	public class TeacherFactory : ITeacherFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public TeacherFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity CreateTeacher()
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isTeacher = true)
				.AddTeacherAttention(TeacherAttention.Writing)
				.AddAlmostCaughtCount(0);
		}
	}
}
