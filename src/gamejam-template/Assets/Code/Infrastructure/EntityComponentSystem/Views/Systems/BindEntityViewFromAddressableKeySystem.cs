using System;
using System.Collections.Generic;
using Code.Infrastructure.EntityComponentSystem.Factories;
using Cysharp.Threading.Tasks;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Views.Systems
{
	public class BindEntityViewFromAddressableKeySystem : IExecuteSystem
	{
		private readonly IEntityViewFactory _viewFactory;
		private readonly IGroup<GameEntity> _entities;
		private readonly List<GameEntity> _buffer = new(32);

		public BindEntityViewFromAddressableKeySystem(GameContext game, IEntityViewFactory viewFactory)
		{
			_viewFactory = viewFactory;
			_entities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.ViewAddressableKey)
				.NoneOf(
					GameMatcher.View,
					GameMatcher.LoadingView));
		}

		public void Execute()
		{
			foreach (GameEntity entity in _entities.GetEntities(_buffer))
			{
				if (string.IsNullOrEmpty(entity.ViewAddressableKey))
					throw new NullReferenceException($"{entity} : {nameof(ViewAddressableKey)} component value is null");

				_viewFactory.CreateViewFromAddressableKey(entity).Forget();
			}
		}
	}
}
