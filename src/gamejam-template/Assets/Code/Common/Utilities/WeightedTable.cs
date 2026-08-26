using System;
using System.Collections.Generic;
using SF = UnityEngine.SerializeField;

namespace Code.Common.Utilities
{
	[Serializable]
	public class WeightedTable<T>
	{
		[SF] private List<WeightedEntry<T>> entries = new();

		public IReadOnlyList<WeightedEntry<T>> Entries => entries;
	}
}
