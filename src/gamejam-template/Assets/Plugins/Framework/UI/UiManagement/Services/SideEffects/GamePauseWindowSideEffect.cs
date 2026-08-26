using System.Collections.Generic;
using Framework.Essentials.TimeManagement;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services.SideEffects
{
	/// <summary>
	/// Holds a pause request for any window whose <see cref="WindowConfig.PausesGame"/> is true.
	/// </summary>
	public class GamePauseWindowSideEffect : IWindowSideEffect
	{
		private readonly ITimeService _timeService;
		private readonly Dictionary<WindowBase, PauseRequestHandler> _handlers = new();

		public GamePauseWindowSideEffect(ITimeService timeService)
		{
			_timeService = timeService;
		}

		public void Apply(WindowBase window, WindowConfig config)
		{
			if (config.PausesGame == false)
				return;

			_handlers[window] = _timeService.RequestPause();
		}

		public void Release(WindowBase window)
		{
			if (_handlers.TryGetValue(window, out PauseRequestHandler handler) == false)
				return;

			_handlers.Remove(window);
			_timeService.ReleasePause(handler);
		}
	}
}
