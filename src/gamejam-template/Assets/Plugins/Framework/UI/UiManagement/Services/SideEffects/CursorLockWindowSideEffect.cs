using System.Collections.Generic;
using Framework.Essentials.CursorManagement;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services.SideEffects
{
	/// <summary>
	/// Holds a cursor lock for any window whose <see cref="WindowConfig.RequiresCursor"/> is true.
	/// </summary>
	public class CursorLockWindowSideEffect : IWindowSideEffect
	{
		private readonly ICursorLockService _cursorLockService;
		private readonly Dictionary<WindowBase, CursorLockStateHandler> _handlers = new();

		public CursorLockWindowSideEffect(ICursorLockService cursorLockService)
		{
			_cursorLockService = cursorLockService;
		}

		public void Apply(WindowBase window, WindowConfig config)
		{
			if (config.RequiresCursor == false)
				return;

			_handlers[window] = _cursorLockService.Request();
		}

		public void Release(WindowBase window)
		{
			if (_handlers.TryGetValue(window, out CursorLockStateHandler handler) == false)
				return;

			_handlers.Remove(window);
			_cursorLockService.Release(handler);
		}
	}
}
