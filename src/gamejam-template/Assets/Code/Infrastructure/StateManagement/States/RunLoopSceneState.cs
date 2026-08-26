using Code.Gameplay;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.StateManagement.Sessions;
using Framework.StateManagement;
using UnityEngine;

namespace Code.Infrastructure.StateManagement.States
{
	public class RunLoopSceneState : IState, IPayloadedEnter<LoopScenePayload>, IExecutable, IFixedExecutable, ILateExecutable, IEndOfFrameExit
	{
		private readonly ISystemFactory _systemFactory;
		private readonly ISessionService _sessionService;

		private CoreLoopFeatureBase _gameplayCoreFeatureBase;
		private FixedUpdateFeature _fixedUpdateFeature;
		private LateUpdateFeature _lateUpdateFeature;
		private LoopNodeId _nodeId;

		public RunLoopSceneState(
			ISystemFactory systemFactory,
			ISessionService sessionService)
		{
			_systemFactory = systemFactory;
			_sessionService = sessionService;
		}

		public void Enter(LoopScenePayload loopScenePayload)
		{
			_nodeId = loopScenePayload.LoopNodeId;

			_fixedUpdateFeature = _systemFactory.Create<FixedUpdateFeature>();
			_lateUpdateFeature = _systemFactory.Create<LateUpdateFeature>();
			_fixedUpdateFeature.Initialize();
			_lateUpdateFeature.Initialize();

			if (RunsAsSession(_nodeId))
			{
				_sessionService.EnterNode(loopScenePayload);
			}
			else
			{
				_gameplayCoreFeatureBase = _systemFactory.Create<LaunchCoreFeature>();
				_gameplayCoreFeatureBase.Initialize();
			}

			Debug.Log($"Run node {_nodeId}, session={RunsAsSession(_nodeId)}");
		}

		public void Exit()
		{
			if (RunsAsSession(_nodeId))
			{
				_sessionService.CloseSession(_nodeId);
			}
			else
			{
				_gameplayCoreFeatureBase.Cleanup();
				_gameplayCoreFeatureBase.TearDown();
				_gameplayCoreFeatureBase = null;
			}

			_fixedUpdateFeature.Cleanup();
			_fixedUpdateFeature.TearDown();
			_fixedUpdateFeature = null;

			_lateUpdateFeature.Cleanup();
			_lateUpdateFeature.TearDown();
			_lateUpdateFeature = null;
		}

		public void Execute()
		{
			if (_gameplayCoreFeatureBase == null)
				return;

			_gameplayCoreFeatureBase.Execute();
			_gameplayCoreFeatureBase.Cleanup();
		}

		public void FixedExecute()
		{
			_fixedUpdateFeature.Execute();
			_fixedUpdateFeature.Cleanup();
		}

		public void LateExecute()
		{
			_lateUpdateFeature.Execute();
			_lateUpdateFeature.Cleanup();
		}

		// Session nodes run inside a branch of the BranchedStateMachine, so several of them can be
		// live at once (gameplay + minigame, for example). Everything else runs as a single pipeline.
		private bool RunsAsSession(LoopNodeId nodeId) =>
			nodeId is LoopNodeId.Battle;
	}
}
