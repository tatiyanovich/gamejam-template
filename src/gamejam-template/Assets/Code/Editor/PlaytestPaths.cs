using System.IO;
using UnityEngine;

namespace Code.Editor
{
	public static class PlaytestPaths
	{
		public static string DirectoryPath => Path.GetFullPath(Path.Combine(Application.dataPath, "../Temp/Playtest"));

		public static string Get(string fileName)
		{
			Directory.CreateDirectory(DirectoryPath);
			return Path.Combine(DirectoryPath, fileName);
		}
	}
}
