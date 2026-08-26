#if UNITY_EDITOR
using UnityEditor.AddressableAssets.Settings;
#endif

namespace Code.Common.Utilities
{
	public static class SceneAddressablesUtilities
	{
#if UNITY_EDITOR
		public static string GetAddressByScenePath(string scenePath)
		{
			if (string.IsNullOrEmpty(scenePath))
				return string.Empty;
			
			string guid = UnityEditor.AssetDatabase.AssetPathToGUID(scenePath);

			if (string.IsNullOrEmpty(guid) == false)
			{
				AddressableAssetSettings settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
				
				if (settings != null)
				{
					AddressableAssetEntry entry = settings.FindAssetEntry(guid);
					
					if (entry != null && string.IsNullOrEmpty(entry.address) == false)
						return entry.address;
				}
			}

			return null;
		}
#endif
	}
}