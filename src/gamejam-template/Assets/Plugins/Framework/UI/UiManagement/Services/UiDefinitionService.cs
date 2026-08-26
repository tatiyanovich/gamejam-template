using System;
using System.Collections.Generic;
using Framework.AssetManagement;
using Framework.UI.UiManagement.Elements.Windows;

namespace Framework.UI.UiManagement.Services
{
	public class UiDefinitionService : IUiDefinitionService
	{
		private readonly IAssetsService _assets;

		private readonly Dictionary<Type, string> _windowDefinitions = new();
		private readonly Dictionary<string, Type> _windowGuidsToType = new();
		private readonly Dictionary<string, WindowConfig> _windowConfigs = new();
		private readonly Dictionary<Type, WindowConfig> _resolvedWindowConfigs = new();

		private readonly Dictionary<Type, string> _widgetDefinitions = new();
		private readonly Dictionary<string, Type> _widgetGuidsToType = new();
		private readonly Dictionary<string, WidgetConfig> _widgetConfigs = new();
		private readonly Dictionary<Type, WidgetConfig> _resolvedWidgetConfigs = new();

		private bool _configsLoaded;

		public UiDefinitionService(IAssetsService assets)
		{
			_assets = assets;
		}

		public void ClearAllDefinitions()
		{
			_windowDefinitions.Clear();
			_windowGuidsToType.Clear();
			_windowConfigs.Clear();
			_resolvedWindowConfigs.Clear();

			_widgetDefinitions.Clear();
			_widgetGuidsToType.Clear();
			_widgetConfigs.Clear();
			_resolvedWidgetConfigs.Clear();

			_configsLoaded = false;
		}

		public IUiDefinitionService AddDefinition(WindowDefinition definition)
		{
			EnsureConfigsLoaded();
			RegisterDefinition(definition.Type, definition.Guid, _windowDefinitions, _windowGuidsToType, "Window");
			return this;
		}

		public IUiDefinitionService AddDefinition(WidgetDefinition definition)
		{
			EnsureConfigsLoaded();
			RegisterDefinition(definition.Type, definition.Guid, _widgetDefinitions, _widgetGuidsToType, "Widget");
			return this;
		}

		public WindowConfig GetWindowConfig<T>() where T : WindowBase => GetWindowConfig(typeof(T));

		public WindowConfig GetWindowConfig(Type windowType)
		{
			EnsureConfigsLoaded();
			return ResolveConfig(windowType, _resolvedWindowConfigs, _windowDefinitions, _windowConfigs, "Window", "OpenWindow");
		}

		public WindowConfig GetWindowConfig(string guid)
		{
			EnsureConfigsLoaded();
			if (_windowConfigs.TryGetValue(guid, out WindowConfig config) == false)
				throw new NotSupportedException($"WindowConfig with guid {guid} is not registered. Tag the asset with the '{UiAddressableLabels.WindowConfigs}' Addressables label.");

			return config;
		}

		public WidgetConfig GetWidgetConfig<T>() where T : WidgetBase => GetWidgetConfig(typeof(T));

		public WidgetConfig GetWidgetConfig(Type widgetType)
		{
			EnsureConfigsLoaded();
			return ResolveConfig(widgetType, _resolvedWidgetConfigs, _widgetDefinitions, _widgetConfigs, "Widget", "OpenWidget");
		}

		public WidgetConfig GetWidgetConfig(string guid)
		{
			EnsureConfigsLoaded();
			if (_widgetConfigs.TryGetValue(guid, out WidgetConfig config) == false)
				throw new NotSupportedException($"WidgetConfig with guid {guid} is not registered. Tag the asset with the '{UiAddressableLabels.WidgetConfigs}' Addressables label.");

			return config;
		}

		private void EnsureConfigsLoaded()
		{
			if (_configsLoaded)
				return;

			LoadConfigs();
		}

		private void LoadConfigs()
		{
			foreach (WindowConfig config in _assets.GetAssetsByLabel<WindowConfig>(UiAddressableLabels.WindowConfigs))
				RegisterConfig(config, config.Guid, _windowConfigs, "WindowConfig");

			foreach (WidgetConfig config in _assets.GetAssetsByLabel<WidgetConfig>(UiAddressableLabels.WidgetConfigs))
				RegisterConfig(config, config.Guid, _widgetConfigs, "WidgetConfig");

			_configsLoaded = true;
		}

		private static void RegisterDefinition(
			Type type,
			string guid,
			Dictionary<Type, string> definitions,
			Dictionary<string, Type> guidsToType,
			string label)
		{
			if (definitions.ContainsKey(type))
				throw new ArgumentException($"{label} definition for {type} is already defined");

			if (guidsToType.TryGetValue(guid, out Type existingType))
				throw new ArgumentException($"Guid {guid} is already used by {label.ToLowerInvariant()} {existingType}");

			definitions.Add(type, guid);
			guidsToType.Add(guid, type);
		}

		private static void RegisterConfig<TConfig>(
			TConfig config,
			string guid,
			Dictionary<string, TConfig> configs,
			string configKind) where TConfig : UnityEngine.Object
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			if (string.IsNullOrEmpty(guid))
				throw new ArgumentException($"{configKind} {config.name} has no guid");

			if (configs.ContainsKey(guid))
				throw new ArgumentException($"{configKind} with guid {guid} is already registered");

			configs.Add(guid, config);
		}

		private static TConfig ResolveConfig<TConfig>(
			Type type,
			Dictionary<Type, TConfig> cache,
			Dictionary<Type, string> definitions,
			Dictionary<string, TConfig> configs,
			string configKind,
			string guidApiExample) where TConfig : UnityEngine.Object
		{
			if (cache.TryGetValue(type, out TConfig cached))
				return cached;

			if (definitions.TryGetValue(type, out string guid) == false)
				throw new NotSupportedException(
					$"{configKind} definition for {type} is not registered. " +
					$"The type-based API requires this — call AddDefinition(new {configKind}Definition(typeof({type.Name}), \"<guid>\")) at startup, " +
					$"or use the guid-based API instead (e.g. {guidApiExample}(\"<guid>\")).");

			if (configs.TryGetValue(guid, out TConfig config) == false)
				throw new NotSupportedException($"{configKind} with guid {guid} for {type} is not registered. Tag the asset with the matching Addressables label.");

			cache.Add(type, config);
			return config;
		}
	}
}
