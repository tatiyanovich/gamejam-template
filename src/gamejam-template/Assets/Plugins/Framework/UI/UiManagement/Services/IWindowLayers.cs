using System.Collections.Generic;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	/// <summary>
	/// Owns the windows-per-layer collection and assigns canvas sorting orders.
	/// Layers are registered once at construction; window membership changes over time.
	/// </summary>
	public interface IWindowLayers
	{
		IReadOnlyList<string> Layers { get; }

		bool IsRegistered(string layerId);
		bool Contains(string layerId, WindowBase window);
		int Count(string layerId);

		void Add(string layerId, WindowBase window);
		void Remove(string layerId, WindowBase window);

		WindowBase Find(string layerId, string configGuid);
		T Find<T>(string layerId) where T : WindowBase;
		WindowBase GetTop(string layerId);

		/// <summary>Returns a fresh copy of the windows in a layer, in stack order (bottom to top).</summary>
		List<WindowBase> Snapshot(string layerId);

		void BringToFront(string layerId, WindowBase window);

		/// <summary>Reassigns canvas sorting order for every window in the layer.</summary>
		void UpdateSorting(string layerId);
	}
}
