using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Views.Systems
{
    public class BindEntityViewFromPathSystem : IExecuteSystem
    {
        private readonly IEntityViewFactory _viewFactory;
        private readonly IGroup<GameEntity> _entities;
        private readonly List<GameEntity> _buffer = new(32);

        public BindEntityViewFromPathSystem(GameContext game, IEntityViewFactory viewFactory)
        {
            _viewFactory = viewFactory;

            _entities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.ViewPath)
                .NoneOf(
                    GameMatcher.View));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _entities.GetEntities(_buffer))
            {
                _viewFactory.CreateViewFromResourcesPath(entity);
            }
        }
    }
}
