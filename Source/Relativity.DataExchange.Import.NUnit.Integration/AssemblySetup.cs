// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;
	using System.Data.SqlClient;
	using System.Threading;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	[SetUpFixture]
	public class AssemblySetup
	{
		public static IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		[OneTimeSetUp]
		public void Setup()
		{
			TestParameters = IntegrationTestHelper.Create();

			int level1GroupId = 1015028;
			int level2GroupId = 1015029;
			int level3GroupId = 1015030;

			_ = UsersHelper.EnsureUser(TestParameters, "Level1", "User", "Level1User!", new[] { level1GroupId }).Result;
			_ = UsersHelper.EnsureUser(TestParameters, "Level2", "User", "Level2User!", new[] { level2GroupId }).Result;
			_ = UsersHelper.EnsureUser(TestParameters, "Level3", "User", "Level3User!", new[] { level3GroupId }).Result;

			List<int> folderIds = FolderHelper.CreateFolders(TestParameters, new[] { "Level1 Permissions", "Level2 Permissions", "Level3 Permissions", "Aaa", "aaa", "Aaa ", "aaa   " }).Result;
			FolderHelper.SetItemLevelSecurity(TestParameters, folderIds[0], "Level 1").Wait();
			FolderHelper.SetItemLevelSecurity(TestParameters, folderIds[1], "Level 2").Wait();
			FolderHelper.SetItemLevelSecurity(TestParameters, folderIds[2], "Level 3").Wait();

			ImportHelper.ImportDefaultTestData(TestParameters);

			if (TestParameters.SqlCaptureProfiling)
			{
				SqlConnectionStringBuilder connectionBuilder = IntegrationTestHelper.GetSqlConnectionStringBuilder(TestParameters);
				SqlProfiling.StartProfilingForRelativityWebApi(connectionBuilder.ConnectionString);
			}
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			if (TestParameters.SqlCaptureProfiling)
			{
				SqlConnectionStringBuilder connectionBuilder = IntegrationTestHelper.GetSqlConnectionStringBuilder(TestParameters);
				string data = SqlProfiling.ReadCapturedEventsAndStopProfiling(connectionBuilder.ConnectionString);
				ProfilingReport profilingReport = SqlProfiling.AggregateReport(data, false);
				Console.WriteLine(profilingReport);
			}

			IntegrationTestHelper.Destroy(TestParameters);
		}
	}
}