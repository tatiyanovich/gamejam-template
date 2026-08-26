using System;
using UnityEditor.TestTools.TestRunner.Api;

namespace Code.Infrastructure.BuildPreprocessors.Editor
{
	public class ResultCollector : ICallbacks
	{
		public ITestResultAdaptor Result { get; private set; }
		public string StackTrace { get; private set; }

		public void RunStarted(ITestAdaptor testsToRun)
		{
			StackTrace = string.Empty;
		}

		public void TestStarted(ITestAdaptor test) { }

		public void TestFinished(ITestResultAdaptor result)
		{
			if (result.ResultState == "Failed")
			{
				StackTrace += $"\n{GetTestName(result)}: {result.Message}";
			}
		}

		public void RunFinished(ITestResultAdaptor result)
		{
			Result = result;
		}

		private string GetTestName(ITestResultAdaptor result)
		{
			try
			{
				string[] testName = result.Name.Split('(');
				return testName[0];
			}
			catch (Exception)
			{
				return result.Name;
			}
		}
	}
}
