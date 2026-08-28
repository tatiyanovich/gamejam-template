using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;

namespace Code.Gameplay.Drilling.Services
{
	public class DrillRunFactory : IDrillRunFactory
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		public DrillRunFactory(IEntityFactory entityFactory, IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;
		}

		public GameEntity CreateRun(float bestDistance)
		{
			return _entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isDrillRun = true)
				.AddDrilledDistance(0f)
				.AddBestDrilledDistance(bestDistance);
		}
	}
}
