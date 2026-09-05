namespace Code.Gameplay.Duck
{
	public static class DuckExtensions
	{
		public static void SwitchDuckState(this GameEntity duck, DuckState state, float timeLeft)
		{
			duck.ReplaceDuckState(state);
			duck.ReplaceDuckStateTimeLeft(timeLeft);
		}

		public static void SettleDuck(this GameEntity duck, DuckState state)
		{
			duck.ReplaceDuckState(state);
			duck.RemoveDuckStateTimeLeft();
		}
	}
}
