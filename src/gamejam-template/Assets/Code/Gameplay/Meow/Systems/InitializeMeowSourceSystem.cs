using Code.Infrastructure.EntityComponentSystem.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Identifiers;
using Entitas;

namespace Code.Gameplay.Meow.Systems
{
	public class InitializeMeowSourceSystem : IInitializeSystem
	{
		private readonly IEntityFactory _entityFactory;
		private readonly IIdentifierService _identifierService;

		private readonly IGroup<GameEntity> _meowSources;

		public InitializeMeowSourceSystem(
			GameContext game,
			IEntityFactory entityFactory,
			IIdentifierService identifierService)
		{
			_entityFactory = entityFactory;
			_identifierService = identifierService;

			_meowSources = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.MeowSource));
		}

		public void Initialize()
		{
			if (_meowSources.count > 0)
				return;

			_entityFactory.Game()
				.AddId(_identifierService.Next())
				.With(x => x.isMeowSource = true)
				.With(x => x.isMeowArmed = true)
				.AddMicrophoneLevel(0f);
		}
	}
}
