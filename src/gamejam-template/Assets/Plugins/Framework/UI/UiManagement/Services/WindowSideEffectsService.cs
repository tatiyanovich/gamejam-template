using System.Collections.Generic;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	public class WindowSideEffectsService : IWindowSideEffectsService
	{
		private readonly List<IWindowSideEffect> _effects;

		public WindowSideEffectsService(List<IWindowSideEffect> effects)
		{
			_effects = effects;
		}

		public void Apply(WindowBase window, WindowConfig config)
		{
			for (int i = 0; i < _effects.Count; i++)
				_effects[i].Apply(window, config);
		}

		public void Release(WindowBase window)
		{
			for (int i = 0; i < _effects.Count; i++)
				_effects[i].Release(window);
		}
	}
}
