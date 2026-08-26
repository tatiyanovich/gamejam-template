using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Infrastructure.StateManagement;
using Code.Infrastructure.StateManagement.Sessions;
using Code.Infrastructure.StateManagement.States;
using Entitas;

namespace Code.Gameplay.CoreLoop.Systems
{
    // A node request is a hard transition: every open session closes and the target scene loads
    // in Single mode. Use a branch request instead when the node should run alongside the others.
    public class HandleLoopNodeRequestSystem : RequestHandlerSystem<GameEntity>
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISessionService _sessionService;

        public HandleLoopNodeRequestSystem(
            GameContext game,
            IGameStateMachine gameStateMachine,
            ISessionService sessionService)
            : base(game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.GoToLoopNodeRequest)))
        {
            _gameStateMachine = gameStateMachine;
            _sessionService = sessionService;
        }

        protected override void OnExecute(IGroup<GameEntity> requests)
        {
            foreach (GameEntity request in requests)
            {
                _sessionService.CloseAll();
                _gameStateMachine.Enter<LoadLoopSceneState, LoopScenePayload>(request.goToLoopNodeRequest.NodeId);
            }
        }
    }
}
