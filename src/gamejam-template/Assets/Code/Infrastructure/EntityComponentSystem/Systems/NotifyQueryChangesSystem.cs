using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Systems
{
	public class NotifyQueryChangesSystem : IExecuteSystem
	{
		private readonly List<IReactiveQuery> _reactiveQueries;

		public NotifyQueryChangesSystem(List<IReactiveQuery> reactiveQueries)
		{
			_reactiveQueries = reactiveQueries;
		}

		public void Execute()
		{
			foreach (IReactiveQuery query in _reactiveQueries)
			{
				query.ReactToChanges();
			}
		}
	}
}
