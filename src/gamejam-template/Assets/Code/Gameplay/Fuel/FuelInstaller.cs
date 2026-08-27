using Code.Gameplay.Fuel.Queries;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.Fuel
{
	public class FuelInstaller : PlainAbstractInstaller
	{
		public FuelInstaller(DiContainer container) : base(container)
		{
		}

		// BindInterfacesTo, not BindInterfacesAndSelfTo: the query is collected as IReactiveQuery.
		public override void InstallBindings() => Container.BindInterfacesTo<FuelQuery>().AsSingle();
	}
}
