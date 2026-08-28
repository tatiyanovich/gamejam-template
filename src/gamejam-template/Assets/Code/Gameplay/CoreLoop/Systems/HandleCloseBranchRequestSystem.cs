using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Systems;
using Code.Infrastructure.StateManagement.Sessions;
using Entitas;

namespace Code.Gameplay.CoreLoop.Systems
{
    public class HandleCloseBranchRequestSystem : RequestHandlerSystem<GameEntity>
    {
        private readonly ISessionService _sessionService;

        private readonly List<GameEntity> _buffer = new(4);

        public HandleCloseBranchRequestSystem(
            GameContext game,
            ISessionService sessionService)
            : base(game.GetGroup(GameMatcher
                .AnyOf(
                    GameMatcher.CloseBranchRequest)))
        {
            _sessionService = sessionService;
        }

        protected override void OnExecute(IGroup<GameEntity> requests)
        {
            foreach (GameEntity request in requests.GetEntities(_buffer))
                _sessionService.CloseSession(request.closeBranchRequest.NodeId);
        }
    }
}
