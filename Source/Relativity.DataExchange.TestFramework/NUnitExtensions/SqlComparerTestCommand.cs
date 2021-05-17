// <copyright file="SqlComparerTestCommand.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System.IO;
	using System.Reflection;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal;
	using NUnit.Framework.Internal.Commands;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.SqlDataComparer;

	internal class SqlComparerTestCommand : BeforeAndAfterTestCommand
	{
		private readonly SqlComparerInputCollector _sqlComparerInputCollector;
		private int _workspaceId;
		private string _workspaceName;
		private TestWorkspaceToCompareDto _testWorkspaceToAdd;

		private int testIteration;

		private int leftInputWorkspaceId;
		private int rightInputWorkspaceId;
		private string leftInputWorkspaceName;
		private string rightInputWorkspaceName;
		private string leftInputFilePath;
		private string rightInputFilePath;

		public SqlComparerTestCommand(TestCommand innerCommand, SqlComparerInputCollector sqlComparerInputCollector)
			: base(innerCommand)
		{
			this._sqlComparerInputCollector = sqlComparerInputCollector;
			this.BeforeTest = ExecuteBeforeTest;
			this.AfterTest = ExecuteAfterTest;
		}

		private void ExecuteBeforeTest(TestExecutionContext context)
		{
			this._workspaceId = IntegrationTestHelper.IntegrationTestParameters.WorkspaceId;
			this._workspaceName = IntegrationTestHelper.IntegrationTestParameters.WorkspaceName;

			string className = context.CurrentTest.TypeInfo.Name;
			string fileName = Path.ChangeExtension(context.CurrentTest.MethodName, "xml");
			string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string comparerConfigFilePath = Path.Combine(rootPath, "Resources", className, fileName);

			string fullTestCaseName = context.CurrentTest.FullName;

			string databaseName = $"EDDS{this._workspaceId}";

			this._testWorkspaceToAdd = new TestWorkspaceToCompareDto(fullTestCaseName, databaseName, comparerConfigFilePath);

			testIteration += 1;
			MassImportImprovementsToggleHelper.SetMassImportImprovementsToggle(IntegrationTestHelper.IntegrationTestParameters, testIteration == 1);
		}

		private void ExecuteAfterTest(TestExecutionContext context)
		{
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Skipped)
			{
				testIteration = 0;
				return;
			}

			string massImportImprovementsToggle =
				MassImportImprovementsToggleHelper.GetDisplayableMassImportImprovementsToggle(IntegrationTestHelper.IntegrationTestParameters);
			string inputFilePath = Path.Combine(IntegrationTestHelper.IntegrationTestParameters.SqlComparerOutputPath, $"sqlComparerInput-{massImportImprovementsToggle}.xml");

			this._sqlComparerInputCollector.AddTestWorkspaceToCompare(this._testWorkspaceToAdd, inputFilePath);
			string testOutputMessage = $"Added test workspace to compare. Test: {this._testWorkspaceToAdd.FullTestCaseName}, database: {this._testWorkspaceToAdd.DatabaseName}, comparerConfig: {this._testWorkspaceToAdd.ComparerConfigFilePath}";
			TestContext.Out.WriteLine(testOutputMessage);

			if (testIteration == 1)
			{
				leftInputWorkspaceId = this._workspaceId;
				this.leftInputWorkspaceName = this._workspaceName;
				this.leftInputFilePath = inputFilePath;
			}
			else if (testIteration == 2)
			{
				rightInputWorkspaceId = this._workspaceId;
				this.rightInputWorkspaceName = this._workspaceName;
				this.rightInputFilePath = inputFilePath;

				var compareConfigPath = Path.Combine(
					IntegrationTestHelper.IntegrationTestParameters.SqlComparerOutputPath,
					$"{context.CurrentTest.MethodName}.xml");

				string compareConfig = File.ReadAllText(compareConfigPath);

				compareConfig = compareConfig.Replace("{leftWorkspaceName}", this.leftInputWorkspaceName);
				compareConfig = compareConfig.Replace("{rightWorkspaceName}", this.rightInputWorkspaceName);

				File.WriteAllText(compareConfigPath, compareConfig);

				string resultFile = Path.Combine(IntegrationTestHelper.IntegrationTestParameters.SqlComparerOutputPath, "sqlComparer_result.txt");

				SqlComparerExecutor.RunSqlComparer(leftInputFilePath, rightInputFilePath, resultFile);
				SqlComparerExecutor.StoreSqlComparerDataForCurrentTest(leftInputFilePath, rightInputFilePath, resultFile, this._testWorkspaceToAdd.ComparerConfigFilePath);

				if (IntegrationTestHelper.IntegrationTestParameters.DeleteWorkspaceAfterTest)
				{
					IntegrationTestHelper.DeleteTestWorkspace(IntegrationTestHelper.IntegrationTestParameters, this.leftInputWorkspaceId);
					IntegrationTestHelper.DeleteTestWorkspace(IntegrationTestHelper.IntegrationTestParameters, this.rightInputWorkspaceId);
				}

				testIteration = 0;
			}
		}
	}
}
