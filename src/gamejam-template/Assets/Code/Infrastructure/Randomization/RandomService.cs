using System.Collections.Generic;

namespace Code.Infrastructure.Randomization
{
	public class RandomService : IRandomService
	{
		private readonly System.Random _random = new();

		public float Value => (float)_random.NextDouble();

		public bool Chance(float probability) => Value < probability;

		public int Range(int min, int maxExclusive) => _random.Next(min, maxExclusive);

		public float Range(float min, float max) => min + (max - min) * Value;

		public T Pick<T>(IList<T> list) => list[Range(0, list.Count)];

		public void Shuffle<T>(IList<T> list)
		{
			for (int i = list.Count - 1; i > 0; i--)
			{
				int j = Range(0, i + 1);
				(list[i], list[j]) = (list[j], list[i]);
			}
		}
	}
}
