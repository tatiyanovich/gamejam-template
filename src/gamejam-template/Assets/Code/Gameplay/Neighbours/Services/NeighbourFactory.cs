using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.Neighbours.Services
{
	public class NeighbourFactory : INeighbourFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public NeighbourFactory(
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity CreateNeighbour(NeighbourSide side)
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isNeighbour = true)
				.AddNeighbourSide(side)
				.AddPawWindowTimeLeft(0f);
		}
	}
}
