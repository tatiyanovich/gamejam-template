using System;
using System.Collections.Generic;

namespace Code.Common.Extensions
{
	public static class CollectionExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (T item in collection)
			{
				action(item);
			}
		}
		
		public static bool Contains<T>(this IEnumerable<T> collection, T item)
		{
			foreach (T candidate in collection)
			{
				if (Equals(candidate, item))
					return true;
			}

			return false;
		}
	}
}