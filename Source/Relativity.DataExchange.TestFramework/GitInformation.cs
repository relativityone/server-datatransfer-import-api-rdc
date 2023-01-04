// ----------------------------------------------------------------------------
// <copyright file="GitInformation.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;

	public static class GitInformation
	{
		private static readonly Lazy<string> GitHash = new Lazy<string>(() => ExecuteCommandAndReturnOutput($"git -C {GetRootFolder()} rev-parse --short HEAD"));
		private static readonly Lazy<string> GitBranch = new Lazy<string>(() => ExecuteCommandAndReturnOutput($"git -C {GetRootFolder()} describe --all"));

		public static string GetCommitHash => GitHash.Value;

		public static string GetGitBranchName => GitBranch.Value;

		private static string GetRootFolder()
		{
			string executionAssembly = Assembly.GetExecutingAssembly().Location;
			return executionAssembly.Substring(0, executionAssembly.IndexOf("Source", StringComparison.CurrentCulture));
		}

		private static string ExecuteCommandAndReturnOutput(string command)
		{
			ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};
			var process = Process.Start(processInfo);

			StreamReader reader = process.StandardOutput;
			string output = reader.ReadToEnd();

			process.WaitForExit(10000);
			process.Close();

			return output.Trim();
		}
	}
}