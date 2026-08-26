using System.Collections.Generic;
using UnityEngine;

namespace Code.Common.Extensions
{
	public static class RendererExtensions
	{
		public static List<Material> GetInstancedMaterials(this Transform root)
		{
			List<Material> instancedMaterials = new();
			Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);

			foreach (Renderer renderer in renderers)
			{
				if (renderer is SpriteRenderer || renderer is ParticleSystemRenderer)
					continue;

				Material[] materials = renderer.materials;
				
				foreach (Material material in materials)
				{
					if (material == null)
						continue;

					instancedMaterials.Add(material);
				}
			}

			return instancedMaterials;
		}
	}
}
