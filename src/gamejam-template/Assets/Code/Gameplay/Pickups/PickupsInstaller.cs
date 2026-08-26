using Code.Gameplay.Pickups.Queries;
using Framework.Essentials.DependencyInjection;
using Zenject;

namespace Code.Gameplay.Pickups
{
	public class PickupsInstaller : PlainAbstractInstaller
	{
		public PickupsInstaller(DiContainer container) : base(container)
		{
		}

		// BindInterfacesTo, not BindInterfacesAndSelfTo: the query is collected as IReactiveQuery.
		public override void InstallBindings() => Container.BindInterfacesTo<ScoreQuery>().AsSingle();
	}
}
