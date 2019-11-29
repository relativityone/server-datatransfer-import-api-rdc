// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Data.SqlClient;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

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