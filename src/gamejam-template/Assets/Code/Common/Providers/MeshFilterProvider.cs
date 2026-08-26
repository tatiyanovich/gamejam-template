using Code.Infrastructure.EntityComponentSystem;
using UnityEngine;
using SF = UnityEngine.SerializeField;

namespace Code.Common.Providers
{
	public class MeshFilterProvider: EntityComponentProvider
	{
		[SF] private MeshFilter meshFilter;

		public override void RegisterComponents() => Entity.AddMeshFilter(meshFilter);
		public override void UnregisterComponents() => Entity.SafeRemoveMeshFilter();
	}
}