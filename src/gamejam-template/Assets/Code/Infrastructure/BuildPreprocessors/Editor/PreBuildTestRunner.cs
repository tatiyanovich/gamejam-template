using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Code.Infrastructure.BuildPreprocessors.Editor
{
	public class PreBuildTestRunner : IPreprocessBuildWithReport
	{
		public int callbackOrder => 1;

		public void OnPreprocessBuild(BuildReport report)
		{
			// RunEditModeTests();
		}

		private void RunEditModeTests()
		{
			ResultCollector resultCollector = new();

			TestRunnerApi api = ScriptableObject.CreateInstance<TestRunnerApi>();
			api.RegisterCallbacks(resultCollector);
			api.Execute(new ExecutionSettings
			{
				runSynchronously = true,
				filters = new[]
				{
					new Filter
					{
						categoryNames = new[] { "Infrastructure", "Gameplay", "Validation" },
						testMode = TestMode.EditMode
					}
				},
			});

			if (resultCollector.Result.FailCount > 0)
				throw new BuildFailedException("Pre-build tests failed!\n" + resultCollector.StackTrace);
			
			Debug.Log("<color=green> All pre-build tests passed! </color>");
		}
	}
}
