using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Events.Extensions
{
	public static class EventGroupExtensions
	{
		/// <summary>
		/// Returns all event-entities that match the given matcher and are ready to be processed.
		/// </summary>
		public static IGroup<GameEntity> GetEvents(this GameContext context, IMatcher<GameEntity> matcher)
		{
			IMatcher<GameEntity> eventMatcher = GameMatcher
				.AllOf(
					matcher, 
					GameMatcher.Event, 
					GameMatcher.Ready);
			
			return context.GetGroup(eventMatcher);
		}
	}
}