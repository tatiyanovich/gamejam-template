using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Systems
{
	public abstract class RequestHandlerSystem<TEntity> : IExecuteSystem 
		where TEntity : Entity
	{
		private readonly IGroup<TEntity> _requests;
		private readonly List<TEntity> _requestBuffer = new(4);
		protected bool ShouldCancelRequestDeletion { get; set; }

		protected RequestHandlerSystem(IGroup<TEntity> requestGroup)
		{
			_requests = requestGroup;
		}

		public void Execute()
		{
			
			if (_requests.count == 0)
				return;

			OnExecute(_requests);

			if (ShouldCancelRequestDeletion)
				return;
			
			foreach (TEntity request in _requests.GetEntities(_requestBuffer))
			{
				request.Destroy();
			}
		}
		
		protected abstract void OnExecute(IGroup<TEntity> requests);
	}
}
