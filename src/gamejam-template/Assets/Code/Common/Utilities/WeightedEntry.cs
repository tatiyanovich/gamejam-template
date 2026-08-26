using System;
using SF = UnityEngine.SerializeField;

namespace Code.Common.Utilities
{
	[Serializable]
	public struct WeightedEntry<T>
	{
		[SF] private T value;
		[SF, UnityEngine.Min(0f)] private float weight;

		public T Value => value;
		public float Weight => weight;
	}
}
