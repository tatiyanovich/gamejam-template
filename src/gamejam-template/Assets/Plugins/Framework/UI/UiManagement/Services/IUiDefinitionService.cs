using System;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	public interface IUiDefinitionService
	{
		void ClearAllDefinitions();

		IUiDefinitionService AddDefinition(WindowDefinition definition);
		IUiDefinitionService AddDefinition(WidgetDefinition definition);

		WindowConfig GetWindowConfig<T>() where T : WindowBase;
		WindowConfig GetWindowConfig(Type windowType);
		WindowConfig GetWindowConfig(string guid);
		WidgetConfig GetWidgetConfig<T>() where T : WidgetBase;
		WidgetConfig GetWidgetConfig(Type widgetType);
		WidgetConfig GetWidgetConfig(string guid);
	}
}
