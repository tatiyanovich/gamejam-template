namespace Framework.UI.UiManagement.Elements.Windows
{
	public enum WidgetLifecycleOwner
	{
		/// <summary>Parent <c>WindowBase</c> calls <c>Open</c>/<c>Close</c>.</summary>
		Window = 0,
		/// <summary><c>OnEnable</c>/<c>OnDisable</c> drive <c>Open</c>/<c>Close</c>.</summary>
		UnityActiveState = 1,
		/// <summary>No automatic lifecycle; must be controlled externally.</summary>
		Manual = 2
	}
}
