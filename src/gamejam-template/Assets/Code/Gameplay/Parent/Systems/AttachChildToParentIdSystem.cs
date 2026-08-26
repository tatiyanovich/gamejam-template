using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Parent.Systems
{
	public sealed class AttachChildToParentIdSystem : IExecuteSystem
	{
		private readonly List<GameEntity> _buffer = new(32);

		private readonly IGroup<GameEntity> _children;
		private readonly IGroup<GameEntity> _parents;

		private readonly GameContext _gameContext;

		public AttachChildToParentIdSystem(GameContext gameContext)
		{
			_gameContext = gameContext;

			_children = gameContext.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Transform,
					GameMatcher.ParentId)
				.NoneOf(
					GameMatcher.ParentAttached));

			_parents = gameContext.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Id,
					GameMatcher.Transform));
		}

		public void Execute()
		{
			foreach (GameEntity child in _children.GetEntities(_buffer))
			{
				GameEntity parent = _gameContext.GetEntityWithId(child.ParentId);

				if (_parents.ContainsEntity(parent) == false)
					continue;

				child.Transform.SetParent(child.Parent, false);
				child.Transform.localPosition = child.hasLocalPosition ? child.LocalPosition : Vector3.zero;
				child.Transform.localRotation = child.hasLocalRotation ? child.LocalRotation : Quaternion.identity;

				child.isParentAttached = true;
			}
		}
	}
}