using Relativity.DataExchange.TestFramework;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Relativity.Desktop.Client.Legacy.Tests.UI
{
	public class TestPathsProvider
	{
		private static readonly string CurrentPath = GetCurrentPath();

		private static readonly string TestInputDirectory =
			GetFullPath(@"..\..\..\..\packages\ui-automation\Relativity.DataExchange.TestData\content");

		private static readonly string TestOutputDirectory = GetFullPath(@"..\..\..\..\TestOutput");
		private static readonly string TestReportsDirectory = TestOutputDirectory.Replace("TestOutput", "TestReports");

		public static string RdcPath(IntegrationTestParameters parameters) => GetFullPath(parameters.RDCPath);

		public string GetTestInputFilePath(string relativePath)
		{
			return Path.Combine(TestInputDirectory, relativePath);
		}

		public string GetTestOutputPath(string relativePath)
		{
			return Path.Combine(TestOutputDirectory, relativePath);
		}

		public static string GetTestReportsDirectory()
		{
			return TestReportsDirectory;
		}

		private static string GetFullPath(string pathFromConfig)
		{
			return IsAbsolutePath(pathFromConfig) ? pathFromConfig : GetAbsolutePath(pathFromConfig);
		}

		private static bool IsAbsolutePath(string path)
		{
			return Path.IsPathRooted(path) && !Path.GetPathRoot(path)
				       .Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
		}

		private static string GetAbsolutePath(string relativePath)
		{
			return Path.GetFullPath(Path.Combine(CurrentPath, relativePath));
		}

		private static string GetCurrentPath()
		{
			return new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
		}
	}
}