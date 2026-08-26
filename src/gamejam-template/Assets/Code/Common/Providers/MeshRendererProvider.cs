using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Common.Providers
{
	public class MeshRendererProvider: EntityComponentProvider
	{
		[SF] private MeshRenderer meshRenderer;

		public override void RegisterComponents() => Entity.AddMeshRenderer(meshRenderer);
		public override void UnregisterComponents() => Entity.SafeRemoveMeshRenderer();
	}
}