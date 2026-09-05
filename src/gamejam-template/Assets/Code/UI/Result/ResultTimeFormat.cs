using UnityEngine;

namespace Code.UI.Result
{
	public static class ResultTimeFormat
	{
		public static string Format(float seconds)
		{
			int total = Mathf.Max(0, Mathf.FloorToInt(seconds));
			return $"{total / 60}:{total % 60:00}";
		}
	}
}
