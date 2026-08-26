using Code.Infrastructure.EntityComponentSystem.Factories;
using Entitas;
using Framework.Essentials.ViewManagement.Services;
using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem.Destruct.Systems
{
	public class CleanupGameDestructedViewSystem : ICleanupSystem
	{
		private readonly IViewPool _viewPool;
		private readonly IEntityViewFactory _viewFactory;
		private readonly IGroup<GameEntity> _entities;

		public CleanupGameDestructedViewSystem(GameContext game, IEntityViewFactory viewFactory, IViewPool viewPool)
		{
			_viewPool = viewPool;
			_viewFactory = viewFactory;

			_entities = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Destructed,
					GameMatcher.View)
				.NoneOf(
					GameMatcher.LoadingView));
		}

		public void Cleanup()
		{
			foreach (GameEntity entity in _entities)
			{
				string viewPath = _viewFactory.GetViewPath(entity);

				if (string.IsNullOrEmpty(viewPath))
				{
					Object.Destroy(entity.View.gameObject);
				}
				else
				{
					_viewPool.Put(entity.View, viewPath);
				}

				entity.View.DetachEntity();
			}
		}
	}
}
