// <copyright file="TestWorkspaceToCompareDto.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.SqlDataComparer
{
	public class TestWorkspaceToCompareDto
	{
		public TestWorkspaceToCompareDto(string fullTestCaseName, string databaseName, string comparerConfigFilePath)
		{
			this.FullTestCaseName = fullTestCaseName;
			this.DatabaseName = databaseName;
			this.ComparerConfigFilePath = comparerConfigFilePath;
		}

		public string FullTestCaseName { get; }

		public string DatabaseName { get; }

		public string ComparerConfigFilePath { get; }
	}
}
