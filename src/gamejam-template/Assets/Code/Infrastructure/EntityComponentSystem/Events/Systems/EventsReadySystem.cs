using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Events.Systems
{
	public class EventsReadySystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _events;

		public EventsReadySystem(GameContext game)
		{
			_events = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Event));
		}

		public void Execute()
		{
			foreach (GameEntity evnt in _events)
			{
				evnt.isReady = true;
			}
		}
	}
}
