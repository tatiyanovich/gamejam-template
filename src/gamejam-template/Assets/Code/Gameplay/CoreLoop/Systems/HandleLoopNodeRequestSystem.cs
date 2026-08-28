using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Infrastructure.StateManagement;
using Code.Infrastructure.StateManagement.Sessions;
using Code.Infrastructure.StateManagement.States;
using Entitas;

namespace Code.Gameplay.CoreLoop.Systems
{
    public class HandleLoopNodeRequestSystem : RequestHandlerSystem<GameEntity>
    {
        private readonly IGameStateMachine _gameStateMachine;
        private readonly ISessionService _sessionService;

        private readonly List<GameEntity> _buffer = new(4);

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
            foreach (GameEntity request in requests.GetEntities(_buffer))
            {
                _sessionService.CloseAll();
                _gameStateMachine.Enter<LoadLoopSceneState, LoopScenePayload>(request.goToLoopNodeRequest.NodeId);
            }
        }
    }
}
