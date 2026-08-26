using Code.Gameplay.Lifetime.Queries;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.Lifetime
{
	public class LifetimeInstaller : PlainAbstractInstaller
	{
		public LifetimeInstaller(DiContainer container) : base(container)
		{
		}

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<LifetimeQuery>().AsSingle();
		}
	}
}