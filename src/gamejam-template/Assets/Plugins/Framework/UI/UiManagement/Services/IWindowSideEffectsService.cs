using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	/// <summary>
	/// Aggregates every registered <see cref="IWindowSideEffect"/> and applies/releases
	/// them as a group when a window opens or closes.
	/// </summary>
	public interface IWindowSideEffectsService
	{
		void Apply(WindowBase window, WindowConfig config);
		void Release(WindowBase window);
	}
}
