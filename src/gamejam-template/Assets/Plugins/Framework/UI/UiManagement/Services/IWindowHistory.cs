namespace Framework.UI.UiManagement.Services
{
	/// <summary>
	/// Per-layer back-navigation history. Windows that get covered (via CloseOnCover)
	/// are pushed here so a later Back() can reopen them. Identified by config guid.
	/// </summary>
	public interface IWindowHistory
	{
		void Push(string layerId, string configGuid);
		bool TryPop(string layerId, out string configGuid);

		/// <summary>Removes every occurrence of the given config guid from the layer's history stack.</summary>
		void RemoveAll(string layerId, string configGuid);
	}
}
