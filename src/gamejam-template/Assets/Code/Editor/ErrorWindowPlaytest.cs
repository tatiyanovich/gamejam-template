using System;
using System.IO;
using System.Text;
using Code.UI.Error;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Code.Editor
{
	public static class ErrorWindowPlaytest
	{
		[MenuItem("COPYCAT/QA/Test error window rendering")]
		public static void Run()
		{
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
				"Assets/AddressableResources/Content/UI/Error/ErrorWindow.prefab");
			GameObject instance = UnityEngine.Object.Instantiate(prefab);
			string reportPath = PlaytestPaths.Get("error-window.txt");
			try
			{
				StringBuilder logs = new();
				logs.AppendLine("NEWEST LOG");
				for (int index = 0; index < 2000; index++)
					logs.AppendLine($"<u><color=red>Exception {index}: nested<T> at SomeMethod()</color></u>");
				logs.Append("END OF LOG");
				ErrorWindow window = instance.GetComponent<ErrorWindow>();
				window.Setup(logs.ToString());
				SerializedObject serialized = new(window);
				TextMeshProUGUI text = (TextMeshProUGUI)serialized.FindProperty("logsText").objectReferenceValue;
				text.ForceMeshUpdate(true);
				if (text.richText || text.text.Length > 12000 || text.text.StartsWith("NEWEST LOG") == false)
					throw new InvalidOperationException("Long logs must be bounded plain text retaining newest entries first.");
				window.Setup("short log");
				text.ForceMeshUpdate(true);
				if (text.text != "short log")
					throw new InvalidOperationException("Short log changed.");
				File.WriteAllText(reportPath, "PASS long marked-up log renders as bounded plain text\nPASS short log\nDONE\n");
			}
			catch (Exception exception)
			{
				File.WriteAllText(reportPath, "FAIL " + exception);
			}
			finally
			{
				UnityEngine.Object.DestroyImmediate(instance);
			}
		}
	}
}
