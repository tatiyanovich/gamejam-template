using System;
using Code.Infrastructure.EntityComponentSystem.Events.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Infrastructure.EntityComponentSystem.Extensions
{
	public static class SystemFactoryExtensions
	{
		public static void UseEvents(this ISystemFactory systemFactory, Feature feature, Action addOtherSystems)
		{
			feature.Add(systemFactory.Create<EventsReadySystem>());
			addOtherSystems?.Invoke();
			feature.Add(systemFactory.Create<EventsCleanupSystem>());
		}
	}
}