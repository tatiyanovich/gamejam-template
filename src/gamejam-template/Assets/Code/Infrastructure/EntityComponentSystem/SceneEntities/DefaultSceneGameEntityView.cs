using UnityEngine;

namespace Code.Infrastructure.EntityComponentSystem.SceneEntities
{
	[RequireComponent(typeof(EntityView))]
	public class DefaultSceneGameEntityView : SceneGameEntityView
	{
		protected override GameEntity BuildEntity() => Game.CreateEntity();
		public override void ResolveReferences() { }
	}
}