using System;
using Code.Infrastructure.EntityComponentSystem.Destruct;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;
using Framework.StateManagement;

namespace Code.Infrastructure.StateManagement.States
{
	public class CleanupAllContextsState : IState, IPayloadedEnter<Action>
	{
		private readonly Contexts _contexts;
		private readonly ISystemFactory _systemFactory;
		private ProcessDestructedFeature _processDestructedFeature;

		public CleanupAllContextsState(
			Contexts contexts,
			ISystemFactory systemFactory)
		{
			_contexts = contexts;
			_systemFactory = systemFactory;
		}

		public void Enter(Action loopScenePayload)
		{
			_processDestructedFeature = _systemFactory.Create<ProcessDestructedFeature>();

			DestroyGameEntities();
			ProcessDestructed();

			DestroyAllEntities();

			loopScenePayload.Invoke();
		}

		private void DestroyGameEntities()
		{
			foreach (GameEntity entity in _contexts.game.GetEntities())
			{
				entity.isDestructed = true;
			}
		}

		private void DestroyAllEntities()
		{
			foreach (IContext context in _contexts.allContexts)
			{
				context.DestroyAllEntities();
			}
		}

		private void ProcessDestructed()
		{
			_processDestructedFeature.Initialize();

			_processDestructedFeature.Execute();
			_processDestructedFeature.Cleanup();

			_processDestructedFeature.DeactivateReactiveSystems();
			_processDestructedFeature.ClearReactiveSystems();

			_processDestructedFeature.TearDown();
			_processDestructedFeature = null;
		}
	}
}