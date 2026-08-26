using Code.Infrastructure.EntityComponentSystem.Events.Extensions;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Pickups.Systems
{
	public class AccumulateScoreSystem : IExecuteSystem
	{
		private readonly IEntityFactory _entityFactory;

		private readonly IGroup<GameEntity> _collectedEvents;
		private readonly IGroup<GameEntity> _scoreHolders;

		public AccumulateScoreSystem(GameContext game, IEntityFactory entityFactory)
		{
			_entityFactory = entityFactory;

			_collectedEvents = game.GetEvents(GameMatcher
				.AllOf(
					GameMatcher.PickupCollectedEvent));

			_scoreHolders = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ScoreHolder,
					GameMatcher.Score));
		}

		public void Execute()
		{
			foreach (GameEntity collectedEvent in _collectedEvents)
			foreach (GameEntity scoreHolder in _scoreHolders)
			{
				scoreHolder.ReplaceScore(scoreHolder.Score + collectedEvent.pickupCollectedEvent.Amount);

				// Persist immediately: the score entity is node-scoped, so leaving the node wipes it
				// before the next auto-save would have run.
				_entityFactory.Request().isSaveProgressRequest = true;
			}
		}
	}
}
