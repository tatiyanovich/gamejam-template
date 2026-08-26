using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Code.Editor
{
	public class ToolbarGenerate
	{
		private const string ToolbarElementName = "ToolbarGenerate/GenerateCode";
		private const string RunJennyMenuItemName = "Tools/Code Generation/Run Jenny";
		private const string RunJennyShortcutId = "Code Generation/Run Jenny";

		[MainToolbarElement(ToolbarElementName, defaultDockPosition = MainToolbarDockPosition.Middle)]
		private static MainToolbarButton CreateGenerateButton()
		{
			Texture2D icon = EditorGUIUtility.FindTexture("d_cs Script Icon");

			return new MainToolbarButton(
				new MainToolbarContent("Generate", icon, "Run Jenny code generation"),
				HandleGenerateClicked);
		}

		private static void HandleGenerateClicked()
		{
			if (CanRunJenny() == false)
				return;

			RunJenny();
		}

		[MenuItem(RunJennyMenuItemName, false, 0)]
		private static void HandleRunJennyMenuItem()
		{
			HandleGenerateClicked();
		}

		[MenuItem(RunJennyMenuItemName, true)]
		private static bool ValidateRunJennyMenuItem()
		{
			return CanRunJenny();
		}

		[Shortcut(RunJennyShortcutId)]
		private static void HandleRunJennyShortcut()
		{
			HandleGenerateClicked();
		}

		private static bool CanRunJenny()
		{
			return Application.isPlaying == false;
		}

		private static void RunJenny()
		{
			string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
			string jennyDir = Path.GetFullPath(Path.Combine(projectRoot, "../../Jenny"));

#if UNITY_EDITOR_WIN
			string batPath = Path.Combine(jennyDir, "Jenny-Gen.bat");
			string comspec = Environment.GetEnvironmentVariable("ComSpec")
			                 ?? @"C:\Windows\System32\cmd.exe";

			ExecuteProcess(
				comspec,
				$"/c \"\"{batPath}\"\"",
				jennyDir
			);
#else
			string scriptPath = Path.Combine(jennyDir, "Jenny-Gen");

			ExecuteProcess(
				"/bin/bash",
				$"\"{scriptPath}\"",
				jennyDir
			);
#endif
		}

		private static void ExecuteProcess(string fileName, string arguments, string workingDirectory)
		{
			try
			{
				ProcessStartInfo processStartInfo = new()
				{
					FileName = fileName,
					Arguments = arguments,
					WorkingDirectory = workingDirectory,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					UseShellExecute = false,
					CreateNoWindow = true
				};

				processStartInfo.EnvironmentVariables["DOTNET_ROLL_FORWARD"] = "Major";

#if UNITY_EDITOR_WIN
				string current = Environment.GetEnvironmentVariable("PATH") ?? "";
				processStartInfo.EnvironmentVariables["PATH"] =
					(@"C:\Program Files\dotnet;" + current);
#endif

				using (Process process = Process.Start(processStartInfo))
				{
					EditorUtility.DisplayProgressBar("Jenny", "Generating…", 0.5f);

					if (process == null)
						throw new Exception("Failed to start process.");

					string stdout = process.StandardOutput.ReadToEnd();
					string stderr = process.StandardError.ReadToEnd();
					process.WaitForExit();

					EditorUtility.ClearProgressBar();

					if (string.IsNullOrEmpty(stdout) == false)
						Debug.Log($"[Jenny] {stdout}");
					if (string.IsNullOrEmpty(stderr) == false)
						Debug.LogError($"[Jenny] {stderr}");

					if (process.ExitCode != 0)
					{
						Debug.LogError($"[Jenny] Exited with code {process.ExitCode}");
					}
					else
					{
						AssetDatabase.Refresh();
						Debug.Log("[Jenny] Generation completed.");
					}
				}
			}
			catch (Exception exception)
			{
				EditorUtility.ClearProgressBar();
				Debug.LogError($"[Jenny] Failed to run: {exception.Message}");
			}
		}
	}
}
