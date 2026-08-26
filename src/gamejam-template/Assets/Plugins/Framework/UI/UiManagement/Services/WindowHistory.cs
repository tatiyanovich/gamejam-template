using System.Collections.Generic;

namespace Framework.UI.UiManagement.Services
{
	public class WindowHistory : IWindowHistory
	{
		private readonly Dictionary<string, Stack<string>> _stacks = new();
		private readonly Stack<string> _scratch = new();

		public WindowHistory(List<string> layerIds)
		{
			foreach (string layerId in layerIds)
				_stacks.Add(layerId, new Stack<string>());
		}

		public void Push(string layerId, string configGuid)
		{
			if (_stacks.TryGetValue(layerId, out Stack<string> stack))
				stack.Push(configGuid);
		}

		public bool TryPop(string layerId, out string configGuid)
		{
			configGuid = null;
			if (_stacks.TryGetValue(layerId, out Stack<string> stack) == false || stack.Count == 0)
				return false;

			configGuid = stack.Pop();
			return true;
		}

		public void RemoveAll(string layerId, string configGuid)
		{
			if (_stacks.TryGetValue(layerId, out Stack<string> stack) == false)
				return;

			_scratch.Clear();
			while (stack.Count > 0)
			{
				string entry = stack.Pop();
				if (entry != configGuid)
					_scratch.Push(entry);
			}

			while (_scratch.Count > 0)
				stack.Push(_scratch.Pop());
		}
	}
}
