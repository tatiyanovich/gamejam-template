using System;

namespace Code.Gameplay.Duck.Queries
{
	public interface IDuckQuery
	{
		event Action<DuckState> OnStateChanged;
		event Action<int> OnThrowCountChanged;

		DuckState GetState();
		int GetThrowCount();
		bool CanThrow();
	}
}
