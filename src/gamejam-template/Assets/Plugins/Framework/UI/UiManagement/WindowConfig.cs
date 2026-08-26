using Framework.Essentials.ScriptableObjects;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.UI.UiManagement
{
	[CreateAssetMenu(menuName = "Framework/UI/Window Config")]
	public class WindowConfig : ScriptableObject
	{
		[SerializeField, ID] private string guid;
		[SerializeField] private AssetReferenceGameObject prefab;
		[SerializeField] private string layerId;
		[SerializeField] private bool closeOnCover;
		[SerializeField] private bool ignoreBack;
		[SerializeField] private bool requiresCursor;
		[SerializeField] private bool pausesGame;

		public string Guid => guid;
		public AssetReferenceGameObject Prefab => prefab;
		public string LayerId => layerId;
		public bool CloseOnCover => closeOnCover;
		public bool IgnoreBack => ignoreBack;
		public bool RequiresCursor => requiresCursor;
		public bool PausesGame => pausesGame;

		/// <summary>Test-only factory. Production code should author configs as ScriptableObject assets.</summary>
		public static WindowConfig CreateForTest(
			string guid,
			string layerId,
			string prefabAssetGuid = null,
			bool closeOnCover = false,
			bool ignoreBack = false,
			bool requiresCursor = false,
			bool pausesGame = false)
		{
			WindowConfig config = CreateInstance<WindowConfig>();
			config.guid = guid;
			config.prefab = new AssetReferenceGameObject(prefabAssetGuid ?? guid);
			config.layerId = layerId;
			config.closeOnCover = closeOnCover;
			config.ignoreBack = ignoreBack;
			config.requiresCursor = requiresCursor;
			config.pausesGame = pausesGame;
			return config;
		}
	}
}
