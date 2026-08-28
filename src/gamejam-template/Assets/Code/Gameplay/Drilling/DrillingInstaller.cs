using Code.Gameplay.Drilling.Queries;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.Drilling
{
	public class DrillingInstaller : PlainAbstractInstaller
	{
		public DrillingInstaller(DiContainer container) : base(container)
		{
		}

		public override void InstallBindings() => Container.BindInterfacesTo<DrillingQuery>().AsSingle();
	}
}
