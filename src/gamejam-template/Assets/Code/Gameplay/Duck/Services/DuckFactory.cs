using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.Duck.Services
{
	public class DuckFactory : IDuckFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public DuckFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity CreateDuck()
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isDuck = true)
				.AddDuckState(DuckState.OnDesk)
				.AddDuckThrowCount(0);
		}

		public void CreateThrowDuckRequest()
		{
			_entityFactory.Request()
				.With(x => x.isThrowDuckRequest = true);
		}
	}
}
