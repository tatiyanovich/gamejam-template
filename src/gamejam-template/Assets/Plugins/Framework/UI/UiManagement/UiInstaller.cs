using System.Collections.Generic;
using Framework.Essentials.DependencyInjection;
using Framework.UI.UiManagement.Services;
using Framework.UI.UiManagement.Services.SideEffects;
using Zenject;

namespace Framework.UI.UiManagement
{
	/// <summary>
	/// Standard installer for UI management.
	/// </summary>
	public class UiInstaller : PlainAbstractInstaller
	{
		private readonly List<string> _uiLayers;
		private readonly IUiHolder _uiHolder;

		/// <param name="container">Zenject container for dependency injection.</param>
		/// <param name="uiLayers">List of UI layers that sorts the windows by "group".</param>
		/// <param name="uiHolder">Holder for UI elements, like UI root.</param>
		public UiInstaller(
			DiContainer container,
			List<string> uiLayers,
			IUiHolder uiHolder) : base(container)
		{
			_uiLayers = uiLayers;
			_uiHolder = uiHolder;
		}

		public override void InstallBindings()
		{
			Container.Bind<IWindowLayers>().To<WindowLayers>().AsSingle().WithArguments(_uiLayers);
			Container.Bind<IWindowHistory>().To<WindowHistory>().AsSingle().WithArguments(_uiLayers);

			Container.Bind<IWindowSideEffect>().To<CursorLockWindowSideEffect>().AsSingle();
			Container.Bind<IWindowSideEffect>().To<GamePauseWindowSideEffect>().AsSingle();
			Container.BindInterfacesTo<WindowSideEffectsService>().AsSingle();

			Container.BindInterfacesTo<UiService>().AsSingle();
			Container.BindInterfacesTo<UiDefinitionService>().AsSingle();
			Container.BindInterfacesTo<UiFactory>().AsSingle();
			Container.Bind<IUiHolder>().FromInstance(_uiHolder).AsSingle();
		}
	}
}
