using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Events.Systems
{
	public class EventsCleanupSystem : ICleanupSystem
	{
		private readonly IGroup<GameEntity> _events;
		private readonly List<GameEntity> _buffer = new(128);

		public EventsCleanupSystem(GameContext game)
		{
			_events = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Event,
					GameMatcher.Ready));
		}

		public void Cleanup()
		{
			foreach (GameEntity evnt in _events.GetEntities(_buffer))
			{
				evnt.Destroy();
			}
		}
	}
}
