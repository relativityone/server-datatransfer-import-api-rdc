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
		private TestWorkspaceToCompareDto _testWorkspaceToAdd;

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

			string className = context.CurrentTest.TypeInfo.Name;
			string fileName = Path.ChangeExtension(context.CurrentTest.MethodName, "xml");
			string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string comparerConfigFilePath = Path.Combine(rootPath, "Resources", className, fileName);

			string fullTestCaseName = context.CurrentTest.FullName;

			string databaseName = $"EDDS{this._workspaceId}";

			this._testWorkspaceToAdd = new TestWorkspaceToCompareDto(fullTestCaseName, databaseName, comparerConfigFilePath);
		}

		private void ExecuteAfterTest(TestExecutionContext context)
		{
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Skipped)
			{
				return;
			}

			bool massImportImprovementsToggle = MassImportImprovementsToggleChecker.GetMassImportToggleValueFromDatabase(IntegrationTestHelper.IntegrationTestParameters);
			string filePath = Path.Combine(
				IntegrationTestHelper.IntegrationTestParameters.SqlComparerOutputPath,
				$"sqlComparerInput-{massImportImprovementsToggle}.xml");
			this._sqlComparerInputCollector.AddTestWorkspaceToCompare(this._testWorkspaceToAdd, filePath);
			string testOutputMessage = $"Added test workspace to compare. Test: {this._testWorkspaceToAdd.FullTestCaseName}, database: {this._testWorkspaceToAdd.DatabaseName}, comparerConfig: {this._testWorkspaceToAdd.ComparerConfigFilePath}";
			TestContext.Out.WriteLine(testOutputMessage);

			// We need to rename a workspace, because it's name is present in the database.
			WorkspaceHelper.RenameTestWorkspace(IntegrationTestHelper.IntegrationTestParameters, this._workspaceId, "ImportApi-SqlComparer");
		}
	}
}
