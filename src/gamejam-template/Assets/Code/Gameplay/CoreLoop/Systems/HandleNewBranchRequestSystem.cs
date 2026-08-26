using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Infrastructure.StateManagement;
using Code.Infrastructure.StateManagement.Sessions;
using Code.Infrastructure.StateManagement.States;
using Entitas;

namespace Code.Gameplay.CoreLoop.Systems
{
    public class HandleNewBranchRequestSystem : RequestHandlerSystem<GameEntity>
    {
        private readonly ISessionService _sessionService;
        private readonly IGameStateMachine _gameStateMachine;

        public HandleNewBranchRequestSystem(
            GameContext game,
            ISessionService sessionService,
            IGameStateMachine gameStateMachine)
            : base(game.GetGroup(GameMatcher
                .AnyOf(
                    GameMatcher.GoToBranchRequest)))
        {
            _sessionService = sessionService;
            _gameStateMachine = gameStateMachine;
        }

        protected override void OnExecute(IGroup<GameEntity> requests)
        {
            foreach (GameEntity request in requests)
            {
                _sessionService.EnterNode(request.goToBranchRequest.NodeId);
            }

            _gameStateMachine.Enter<SessionsRunningState>();
        }
    }
}