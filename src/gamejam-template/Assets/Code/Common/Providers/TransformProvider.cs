using Code.Infrastructure.EntityComponentSystem;

namespace Code.Common.Providers
{
	public class TransformProvider : EntityComponentProvider
	{
		public override void RegisterComponents() => Entity.ReplaceTransform(transform);
		public override void UnregisterComponents() => Entity.SafeRemoveTransform();
	}
}