using System.Collections.Generic;
using System.Linq;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	public class WindowLayers : IWindowLayers
	{
		private const int SortingOrderPerLayer = 100;

		private readonly Dictionary<string, List<WindowBase>> _windowsPerLayer = new();
		private readonly Dictionary<string, int> _layerIndices = new();
		private readonly List<string> _layerIds = new();

		public WindowLayers(List<string> layerIds)
		{
			for (int i = 0; i < layerIds.Count; i++)
			{
				string layerId = layerIds[i];
				_windowsPerLayer.Add(layerId, new List<WindowBase>());
				_layerIndices.Add(layerId, i);
				_layerIds.Add(layerId);
			}
		}

		public IReadOnlyList<string> Layers => _layerIds;

		public bool IsRegistered(string layerId)
		{
			return _windowsPerLayer.ContainsKey(layerId);
		}

		public bool Contains(string layerId, WindowBase window)
		{
			return _windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list) && list.Contains(window);
		}

		public int Count(string layerId)
		{
			return _windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list) ? list.Count : 0;
		}

		public void Add(string layerId, WindowBase window)
		{
			if (_windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list))
				list.Add(window);
		}

		public void Remove(string layerId, WindowBase window)
		{
			if (_windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list) && list.Remove(window))
				UpdateSorting(layerId);
		}

		public WindowBase Find(string layerId, string configGuid)
		{
			return _windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list)
				? list.LastOrDefault(w => w.ConfigGuid == configGuid)
				: null;
		}

		public T Find<T>(string layerId) where T : WindowBase
		{
			return _windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list)
				? list.OfType<T>().LastOrDefault()
				: null;
		}

		public WindowBase GetTop(string layerId)
		{
			return _windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list)
				? list.LastOrDefault()
				: null;
		}

		public List<WindowBase> Snapshot(string layerId)
		{
			return _windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list)
				? new List<WindowBase>(list)
				: new List<WindowBase>();
		}

		public void BringToFront(string layerId, WindowBase window)
		{
			if (_windowsPerLayer.TryGetValue(layerId, out List<WindowBase> list) == false)
				return;

			list.Remove(window);
			list.Add(window);
			UpdateSorting(layerId);
		}

		public void UpdateSorting(string layerId)
		{
			if (_layerIndices.TryGetValue(layerId, out int layerIndex) == false)
				return;

			List<WindowBase> list = _windowsPerLayer[layerId];
			for (int i = 0; i < list.Count; i++)
			{
				if(list[i].OverridingSortingOrder)
					continue;
				
				list[i].Canvas.overrideSorting = true;
				list[i].Canvas.sortingOrder = layerIndex * SortingOrderPerLayer + i;
			}
		}
	}
}
