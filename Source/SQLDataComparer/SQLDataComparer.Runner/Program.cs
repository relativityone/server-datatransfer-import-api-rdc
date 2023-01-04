using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using SQLDataComparer.Runner.InputDto;

namespace SQLDataComparer.Runner
{
	class Program
	{
		static int Main(string[] args)
		{
			if (args.Length < 5)
			{
				Console.WriteLine("Usage: SQLDataComparer.Runner.exe sqlServer username password leftInput rightInput");
				return 0;
			}

			string server = args[0];
			string username = args[1];
			string password = args[2];
			string leftInputPath = args[3];
			string rightInputPath = args[4];

			string inputRootDirectory = Path.GetDirectoryName(leftInputPath);
			Input leftInput = ReadInput(leftInputPath);
			Input rightInput = ReadInput(rightInputPath);

			var sqlDataComparer = new SqlDataComparer(server, username, password);

			var nonIdenticalTestWorkspaces = new List<TestWorkspace>();
			foreach (TestWorkspace leftTestWorkspace in leftInput.TestWorkspaces)
			{
				TestWorkspace rightTestWorkspace = rightInput
						.TestWorkspaces
						.FirstOrDefault(x => x.TestName == leftTestWorkspace.TestName);

				if (rightTestWorkspace == null)
				{
					nonIdenticalTestWorkspaces.Add(leftTestWorkspace);
					Console.WriteLine($"Second database does not exist for test: {leftTestWorkspace.TestName}");
				}
				else
				{
					string comparerConfigFilePath = Path.Combine(inputRootDirectory, leftTestWorkspace.ComparerConfigFilePath);
					Console.WriteLine($"Test: {leftTestWorkspace.TestName} - {leftTestWorkspace.DatabaseName} - {rightTestWorkspace.DatabaseName}");
					bool areIdentical = sqlDataComparer.Run(comparerConfigFilePath, leftTestWorkspace.DatabaseName, rightTestWorkspace.DatabaseName);
					if (!areIdentical)
					{
						nonIdenticalTestWorkspaces.Add(leftTestWorkspace);
					}
				}
			}

			Console.WriteLine("Comparing tests databases completed.");
			if (nonIdenticalTestWorkspaces.Any())
			{
				Console.WriteLine("Non-identical workspaces:");
				foreach (var testWorkspace in nonIdenticalTestWorkspaces)
				{
					Console.WriteLine($"{testWorkspace.DatabaseName} - {testWorkspace.TestName}");
				}

				return 1;
			}

			Console.WriteLine("All workspaces are identical.");
			return 0;
		}

		private static Input ReadInput(string inputPath)
		{
			var xmlSerializer = new XmlSerializer(typeof(Input));
			using (var sr = File.OpenRead(inputPath))
			{
				return (Input)xmlSerializer.Deserialize(sr);
			}
		}
	}
}
