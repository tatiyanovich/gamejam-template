using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

#if UNITY_2019_1_OR_NEWER

#else
using UnityEngine.Experimental.UIElements;
#endif

namespace Code.Editor
{
    public static class ToolbarCallback
    {
        private static readonly Type toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        private static readonly Type guiViewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
#if UNITY_2020_1_OR_NEWER
        private static readonly Type windowBackendType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.IWindowBackend");

        private static readonly PropertyInfo windowBackend = guiViewType.GetProperty("windowBackend", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static readonly PropertyInfo viewVisualTree = windowBackendType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
		private static readonly PropertyInfo viewVisualTree = m_guiViewType.GetProperty("visualTree", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        private static readonly FieldInfo guiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        private static ScriptableObject currentToolbar;

        public static Action onToolbarGUI;
        public static Action onToolbarGUILeft;
        public static Action onToolbarGUIRight;

        static ToolbarCallback()
        {
            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            if (currentToolbar != null)
                return;
            
            Object[] toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;

            if (currentToolbar == null) 
                return;
            
#if UNITY_2021_1_OR_NEWER
            FieldInfo rootField = currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            object rawRoot = rootField?.GetValue(currentToolbar);
            VisualElement root = rawRoot as VisualElement;
                    
            registerCallback("ToolbarZoneLeftAlign", onToolbarGUILeft);
            registerCallback("ToolbarZoneRightAlign", onToolbarGUIRight);

            void registerCallback(string r, Action callback)
            {
                VisualElement toolbarZone = root.Q(r);

                VisualElement parent = new()
                {
                    style =
                    {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };
                        
                IMGUIContainer container = new();
                container.style.flexGrow = 1;
                container.onGUIHandler += () => { callback?.Invoke(); };
                parent.Add(container);
                toolbarZone.Add(parent);
            }
#else
#if UNITY_2020_1_OR_NEWER
					object backend = windowBackend.GetValue(currentToolbar);
					VisualElement visualTree = (VisualElement) viewVisualTree.GetValue(backend, null);
#else
					VisualElement visualTree = (VisualElement) viewVisualTree.GetValue(currentToolbar, null);
#endif
					IMGUIContainer container = (IMGUIContainer) visualTree[0];

					Action handler = (Action) guiContainerOnGui.GetValue(container);
					handler -= OnGUI;
					handler += OnGUI;
					guiContainerOnGui.SetValue(container, handler);
#endif
        }

        private static void OnGUI()
        {
            Action handler = onToolbarGUI;
            
            if (handler != null) 
                handler();
        }
    }
}