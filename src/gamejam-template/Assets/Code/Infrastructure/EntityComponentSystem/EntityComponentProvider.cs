namespace Code.Infrastructure.EntityComponentSystem
{
	public abstract class EntityComponentProvider : EntityDependant
	{
		public abstract void RegisterComponents();
		public abstract void UnregisterComponents();
	}
}