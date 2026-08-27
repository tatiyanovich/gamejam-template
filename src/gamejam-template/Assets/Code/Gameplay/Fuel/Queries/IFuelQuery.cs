using System;

namespace Code.Gameplay.Fuel.Queries
{
	public interface IFuelQuery
	{
		event Action<float, float> OnFuelChanged;
		float GetFuel();
		float GetMaxFuel();
	}
}
