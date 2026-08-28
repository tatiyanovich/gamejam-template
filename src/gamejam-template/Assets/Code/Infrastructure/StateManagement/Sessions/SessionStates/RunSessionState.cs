using System;
using Code.Gameplay;
using Code.Infrastructure.CoreLoop;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Code.Infrastructure.Scenes;
using Code.Infrastructure.StateManagement.States;
using Cysharp.Threading.Tasks;
using Framework.Essentials.SceneManagement;
using Framework.StateManagement;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Code.Infrastructure.StateManagement.Sessions.SessionStates
{
	public class RunSessionState : IState, IPayloadedEnter<LoopScenePayload>, IExecutable, IExit
	{
		private readonly ISystemFactory _systemFactory;
		private readonly ISessionRevealGate _revealGate;
		private readonly ISessionWindowsPresenter _windowsPresenter;
		private readonly ISceneLoadService _sceneLoadService;
		private readonly ILoadedSceneRegistry _loadedSceneRegistry;
		private readonly ILoopNodeContext _loopNodeContext;

		private Feature _runFeature;
		private LoopNodeId _nodeId;

		private const float RevealDelay = 1f;

		public RunSessionState(
			ISystemFactory systemFactory,
			ISessionRevealGate revealGate,
			ISessionWindowsPresenter windowsPresenter,
			ISceneLoadService sceneLoadService,
			ILoadedSceneRegistry loadedSceneRegistry,
			ILoopNodeContext loopNodeContext)
		{
			_systemFactory = systemFactory;
			_revealGate = revealGate;
			_windowsPresenter = windowsPresenter;
			_sceneLoadService = sceneLoadService;
			_loadedSceneRegistry = loadedSceneRegistry;
			_loopNodeContext = loopNodeContext;
		}

		public void Enter(LoopScenePayload loopScenePayload)
		{
			_nodeId = loopScenePayload.LoopNodeId;

			if (_runFeature == null)
			{
				_runFeature = GetTargetLoopFeature();
				_loopNodeContext.Current = _nodeId;
				try
				{
					_runFeature.Initialize();
				}
				finally
				{
					_loopNodeContext.Current = LoopNodeId.Unknown;
				}
			}

			UnloadLaunchScene();

			_revealGate.SetNextRevealDelay(RevealDelay);
			_revealGate.NotifyReady();

			Debug.Log($"[Run Session State] Session with type {_runFeature.GetType()} is running.");

			Feature GetTargetLoopFeature()
			{
				switch (loopScenePayload.LoopNodeId)
				{
					case LoopNodeId.Battle:
						return _systemFactory.Create<GameplayCoreFeature>();
					default:
						throw new NotImplementedException(
							$"No core feature is mapped to session node {loopScenePayload.LoopNodeId}");
				}
			}
		}

		public void Execute()
		{
			_runFeature.Execute();
			_runFeature.Cleanup();
		}

		public void Exit()
		{
			_windowsPresenter.Dismiss(_nodeId).Forget();

			if (LoopNodes.IsPersistent(_nodeId))
				return;

			Debug.Log($"[Run Session State] state with type {_runFeature.GetType()} was torn down.");

			_runFeature.Cleanup();
			_runFeature.TearDown();
			_runFeature = null;

			UnloadSessionScene();
		}

		private void UnloadSessionScene()
		{
			if (_loadedSceneRegistry.TryGet(_nodeId, out SceneInstance scene) == false)
				return;

			_loadedSceneRegistry.Remove(_nodeId);
			_sceneLoadService.UnloadScene(scene).Forget();
		}

		private void UnloadLaunchScene()
		{
			if (_loadedSceneRegistry.TryGet(LoopNodeId.StartLaunch, out SceneInstance launchScene) == false)
				return;

			_loadedSceneRegistry.Remove(LoopNodeId.StartLaunch);
			_sceneLoadService.UnloadScene(launchScene).Forget();
		}
	}
}
