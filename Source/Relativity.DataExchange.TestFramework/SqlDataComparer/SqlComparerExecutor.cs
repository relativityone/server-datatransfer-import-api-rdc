// ----------------------------------------------------------------------------
// <copyright file="SqlComparerExecutor.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------
namespace Relativity.DataExchange.TestFramework.SqlDataComparer
{
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using NUnit.Framework;
	using File = System.IO.File;

	public static class SqlComparerExecutor
	{
		public static void RunSqlComparer(string leftInputFile, string rightInputFile, string resultFile)
		{
			const int SqlComparerTimeoutInMs = 120000;
			string sourcesFolder = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory.Parent.Parent.FullName;
			string sqlComparerRunner = Path.Combine(sourcesFolder, "SQLDataComparer", "SQLDataComparer.Runner", "bin", "Release", "SQLDataComparer.Runner.exe");

			string command = $"{IntegrationTestHelper.IntegrationTestParameters.SqlInstanceName} "
			                 + $"{IntegrationTestHelper.IntegrationTestParameters.SqlAdminUserName} "
			                 + $"{IntegrationTestHelper.IntegrationTestParameters.SqlAdminPassword} "
			                 + $"{leftInputFile} "
			                 + $"{rightInputFile}";

			ProcessStartInfo processInfo = new ProcessStartInfo($"{sqlComparerRunner}", command)
				                               {
					                               CreateNoWindow = true,
					                               UseShellExecute = false,
					                               RedirectStandardOutput = true,
				                               };
			var process = Process.Start(processInfo);

			StreamReader reader = process.StandardOutput;
			string output = reader.ReadToEnd();

			process.WaitForExit(SqlComparerTimeoutInMs);
			process.Close();

			File.WriteAllText(resultFile, output);
		}

		public static void StoreSqlComparerDataForCurrentTest(string leftInputFile, string rightInputFile, string resultFile, string comparerConfigFile)
		{
			string testFolder = Path.Combine(
				IntegrationTestHelper.IntegrationTestParameters.SqlComparerOutputPath,
				TestContext.CurrentContext.Test.Name).Replace(",", "_");

			Directory.CreateDirectory(testFolder);

			MoveFile(Path.Combine(testFolder, Path.GetFileName(leftInputFile)), leftInputFile);
			MoveFile(Path.Combine(testFolder, Path.GetFileName(rightInputFile)), rightInputFile);
			MoveFile(Path.Combine(testFolder, Path.GetFileName(resultFile)), resultFile);

			File.Copy(comparerConfigFile, Path.Combine(testFolder, Path.GetFileName(comparerConfigFile)), true);
		}

		private static void MoveFile(string destinationFile, string inputFile)
		{
			if (File.Exists(destinationFile))
			{
				File.Delete(destinationFile);
			}

			File.Move(inputFile, destinationFile);
		}
	}
}
