using Code.Gameplay.Input;
using Code.Infrastructure.EntityComponentSystem.Events.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Infrastructure.EntityComponentSystem.Views;

namespace Code.Gameplay.CoreLoop
{
	/// <summary>
	/// Feature with general system which exectue BEFORE game pipline features. Be patient this feature cant manage cleanup systems
	/// </summary>
	public sealed class GlobalLoopInfraHeadFeature : Feature
	{
		public GlobalLoopInfraHeadFeature(ISystemFactory systemFactory)
		{
			Add(systemFactory.Create<InputFeature>());

			Add(systemFactory.Create<EventsReadySystem>());
			Add(systemFactory.Create<MarkStaleRequestsSystem>());
			Add(systemFactory.Create<ViewFeature>());
		}
	}
}
