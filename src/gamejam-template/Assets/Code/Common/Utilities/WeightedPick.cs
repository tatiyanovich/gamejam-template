using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Common.Utilities
{
	public static class WeightedPick
	{
		public static T Pick<T>(WeightedTable<T> table, Func<WeightedEntry<T>, float> weightSelector, double roll01)
		{
			return Pick(table.Entries, weightSelector, roll01).Value;
		}

		public static T Pick<T>(IReadOnlyList<T> items, Func<T, float> weightSelector, double roll01)
		{
			float totalWeight = 0f;

			foreach (T item in items)
				totalWeight += Mathf.Max(0f, weightSelector(item));

			if (totalWeight <= 0f)
				return items[0];

			double target = roll01 * totalWeight;
			float cumulative = 0f;

			foreach (T item in items)
			{
				cumulative += Mathf.Max(0f, weightSelector(item));

				if (target <= cumulative)
					return item;
			}

			return items[items.Count - 1];
		}
	}
}
