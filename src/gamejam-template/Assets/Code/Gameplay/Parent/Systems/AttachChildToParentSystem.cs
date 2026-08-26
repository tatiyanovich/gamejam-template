using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Parent.Systems
{
	public sealed class AttachChildToParentSystem : IExecuteSystem
	{
		private readonly IGroup<GameEntity> _children;
		private readonly List<GameEntity> _buffer = new(32);

		public AttachChildToParentSystem(GameContext game)
		{
			_children = game.GetGroup(GameMatcher
				.AllOf(
					GameMatcher.Transform,
					GameMatcher.Parent)
				.NoneOf(
					GameMatcher.ParentAttached));
		}

		public void Execute()
		{
			foreach (GameEntity child in _children.GetEntities(_buffer))
			{
				child.Transform.SetParent(child.Parent, false);
				child.Transform.localPosition = Vector3.zero;
				child.Transform.localRotation = Quaternion.identity;
				
				child.isParentAttached = true;
			}
		}
	}
}
