namespace Framework.UI.UiManagement
{
	/// <summary>
	/// Addressables labels the UI framework scans at startup to discover configs.
	/// Tag every <see cref="WindowConfig"/> asset with <see cref="WindowConfigs"/>
	/// and every <see cref="WidgetConfig"/> asset with <see cref="WidgetConfigs"/>.
	/// </summary>
	public static class UiAddressableLabels
	{
		public const string WindowConfigs = "window_configs";
		public const string WidgetConfigs = "widget_configs";
	}
}
