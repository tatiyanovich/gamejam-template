using Framework.Essentials.DependencyInjection;
using Framework.Essentials.ViewManagement.Services;
using Zenject;

namespace Framework.Essentials.ViewManagement
{
	public class ViewManagementInstaller : PlainAbstractInstaller
	{
		public ViewManagementInstaller(DiContainer container) : base(container)
		{
		}

		public override void InstallBindings()
		{
			Container.BindInterfacesTo<ViewFactory>().AsSingle();
			Container.BindInterfacesTo<ViewPool>().AsSingle();
		}
	}
}
