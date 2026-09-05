using UnityEngine;

namespace Code.Gameplay.Suspicion
{
	public static class SuspicionExtensions
	{
		public static void ChangeSuspicion(this GameEntity run, float delta, float maximumLevel)
		{
			run.ReplaceSuspicionLevel(Mathf.Clamp(run.SuspicionLevel + delta, 0f, maximumLevel));
		}
	}
}
