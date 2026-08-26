using System;

namespace Code.Infrastructure.AppMetadata.Snapshots
{
	[Serializable]
	public class AppMetadataSnapshot
	{
		public long FirstLaunchUnixTime;
		public string LastUsedVersion;
		public int LastUsedId;
		
		public AppMetadataSnapshot(long firstLaunchUnixTime, string lastUsedVersion, int lastUsedId)
		{
			FirstLaunchUnixTime = firstLaunchUnixTime;
			LastUsedVersion = lastUsedVersion;
			LastUsedId = lastUsedId;
		}
	}
}