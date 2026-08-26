using System;
using Code.Infrastructure.Scenes;
using Code.Infrastructure.StateManagement.Branching;
using Code.Infrastructure.StateManagement.States;
using Code.UI.Fade;
using Cysharp.Threading.Tasks;
using Framework.Essentials.SceneManagement;
using Framework.StateManagement;
using Framework.UI.UiManagement.Services;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Code.Infrastructure.StateManagement.Sessions.SessionStates
{
    public class PreloadSessionState : IState, IPayloadedEnter<LoopScenePayload>
    {
        private readonly IBranchedStateMachine _branchedStateMachine;
        private readonly ISceneLoadService _sceneLoadService;
        private readonly ILoadedSceneRegistry _loadedSceneRegistry;
        private readonly IUiService _uiService;
        private readonly ISessionWindowsPresenter _windowsPresenter;
        private readonly ISessionRevealGate _revealGate;

        public PreloadSessionState(
            IBranchedStateMachine branchedStateMachine,
            ISceneLoadService sceneLoadService,
            ILoadedSceneRegistry loadedSceneRegistry,
            IUiService uiService,
            ISessionWindowsPresenter windowsPresenter,
            ISessionRevealGate revealGate)
        {
            _branchedStateMachine = branchedStateMachine;
            _sceneLoadService = sceneLoadService;
            _loadedSceneRegistry = loadedSceneRegistry;
            _uiService = uiService;
            _windowsPresenter = windowsPresenter;
            _revealGate = revealGate;
        }

        public void Enter(LoopScenePayload loopScenePayload)
        {
            _revealGate.RegisterPending();

            Load(loopScenePayload).Forget();
        }

        private async UniTaskVoid Load(LoopScenePayload loopScenePayload)
        {
            FadeWindow fadeWindow = await _uiService.OpenWindow<FadeWindow>(withAnimation: false);
            await fadeWindow.FadeIn(1);

            await _windowsPresenter.Present(loopScenePayload.LoopNodeId);

            if (_loadedSceneRegistry.TryGet(loopScenePayload.LoopNodeId, out _))
            {
                ChooseNextState(loopScenePayload);
                return;
            }

            SceneInstance scene = await _sceneLoadService.LoadScene(
                loopScenePayload.SceneAddress,
                LoadSceneMode.Additive,
                onLoaded: () => ChooseNextState(loopScenePayload));

            _loadedSceneRegistry.Register(loopScenePayload.LoopNodeId, scene);
        }

        private void ChooseNextState(LoopScenePayload loopScenePayload)
        {
            if (_branchedStateMachine.TryGetBranch(loopScenePayload.LoopNodeId, out IStateBranch branch))
                branch.Enter<RunSessionState, LoopScenePayload>(loopScenePayload);
            else
                throw new NotImplementedException($"Can't load next node it session {loopScenePayload.LoopNodeId}");
        }
    }
}
