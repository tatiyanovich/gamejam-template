using System;

namespace Framework.Essentials.ScriptableObjects
{
	public static class IDUtility
	{
		public static string GenerateID() => Guid.NewGuid().ToString();
	}
}
