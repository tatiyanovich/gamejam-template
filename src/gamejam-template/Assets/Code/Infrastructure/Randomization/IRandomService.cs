using System.Collections.Generic;

namespace Code.Infrastructure.Randomization
{
	public interface IRandomService
	{
		float Value { get; }
		bool Chance(float probability);
		int Range(int min, int maxExclusive);
		float Range(float min, float max);
		T Pick<T>(IList<T> list);
		void Shuffle<T>(IList<T> list);
	}
}
