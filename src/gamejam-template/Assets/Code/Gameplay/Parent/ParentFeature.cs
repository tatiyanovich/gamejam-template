using Code.Gameplay.Parent.Systems;
using Code.Infrastructure.EntityComponentSystem.Factories;

namespace Code.Gameplay.Parent
{
	public sealed class ParentFeature : Feature
	{
		public ParentFeature(ISystemFactory systems)
		{
			Add(systems.Create<AttachChildToParentSystem>());
			Add(systems.Create<AttachChildToParentIdSystem>());
		}
	}
}