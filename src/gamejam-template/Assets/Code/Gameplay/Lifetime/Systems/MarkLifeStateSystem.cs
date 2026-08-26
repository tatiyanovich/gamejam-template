using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;

namespace Code.Gameplay.Lifetime.Systems
{
    public class MarkLifeStateSystem : IExecuteSystem
    {
        private readonly IEntityFactory _entityFactory;
        private readonly List<GameEntity> _buffer = new(64);
        private readonly IGroup<GameEntity> _healthChangedEntities;
        private readonly IGroup<GameEntity> _noHealthStateEntities;

        public MarkLifeStateSystem(GameContext game, IEntityFactory entityFactory)
        {
            _entityFactory = entityFactory;

            _healthChangedEntities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Id,
                    GameMatcher.CurrentHP,
                    GameMatcher.CurrentHPChanged));

            _noHealthStateEntities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Id,
                    GameMatcher.CurrentHP)
                .NoneOf(
                    GameMatcher.Alive,
                    GameMatcher.Dead));
        }

        public void Execute()
        {
            foreach (GameEntity entity in _healthChangedEntities.GetEntities(_buffer))
            {
                ProcessHealthState(entity);
            }

            foreach (GameEntity entity in _noHealthStateEntities.GetEntities(_buffer))
            {
                ProcessHealthState(entity);
            }
        }

        private void ProcessHealthState(GameEntity entity)
        {
            bool wasDead = entity.isDead;

            entity.isAlive = entity.CurrentHP > 0;
            entity.isDead = entity.CurrentHP <= 0;

            if (wasDead == false && entity.isDead)
            {
                _entityFactory.Event().AddDeathEvent(entity.Id);
            }
        }
    }
}