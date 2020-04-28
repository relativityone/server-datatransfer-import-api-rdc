﻿// <copyright file="CollectDeadlocksAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal.Commands;

	using Relativity.DataExchange.TestFramework.WebApiSqlProfiling.DeadlockReport;

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class CollectDeadlocksAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		public TestCommand Wrap(TestCommand command)
		{
			return IntegrationTestHelper.IntegrationTestParameters.SqlCaptureProfiling
					   ? CreateTestCommandWithProfiling(command)
					   : command;
		}

		private static TestCommand CreateTestCommandWithProfiling(TestCommand command)
		{
			string connectionString = IntegrationTestHelper.GetSqlConnectionStringBuilder().ConnectionString;
			string outputPath = IntegrationTestHelper.IntegrationTestParameters.SqlProfilingReportsOutputPath;

			return new ProfilingSessionTestCommand(command, new CollectDeadlockProfilerReportSession(connectionString), outputPath);
		}
	}
}