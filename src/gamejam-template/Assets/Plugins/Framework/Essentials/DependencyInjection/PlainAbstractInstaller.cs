using Zenject;

namespace Framework.Essentials.DependencyInjection
{
	/// <summary>
	/// Provides an easy way to bind a set of services without creating a child container.
	/// Usage: <c>new SomeInstaller(Container).InstallBindings();</c>
	/// </summary>
	public abstract class PlainAbstractInstaller
	{
		protected readonly DiContainer Container;

		protected PlainAbstractInstaller(DiContainer container)
		{
			Container = container;
		}

		public abstract void InstallBindings();
	}
}
