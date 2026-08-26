using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	/// <summary>
	/// A single ambient effect tied to a window's open lifetime — e.g. cursor lock,
	/// game pause, music ducking. Implementations decide their own activation criteria
	/// and own any per-window state needed to release the effect later.
	/// </summary>
	public interface IWindowSideEffect
	{
		void Apply(WindowBase window, WindowConfig config);
		void Release(WindowBase window);
	}
}
