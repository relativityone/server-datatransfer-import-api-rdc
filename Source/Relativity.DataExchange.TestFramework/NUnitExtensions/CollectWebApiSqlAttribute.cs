// <copyright file="CollectWebApiSqlAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal.Commands;

	using Relativity.DataExchange.TestFramework.WebApiSqlProfiling.SqlStatement;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CollectWebApiSqlAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		public TestCommand Wrap(TestCommand command)
		{
			return IntegrationTestHelper.ReadIntegrationTestParameters().SqlCaptureProfiling
					   ? CreateTestCommandWithProfiling(command)
					   : command;
		}

		private static TestCommand CreateTestCommandWithProfiling(TestCommand command)
		{
			string connectionString = IntegrationTestHelper.GetSqlConnectionStringBuilder().ConnectionString;
			string outputPath = IntegrationTestHelper.ReadIntegrationTestParameters().SqlProfilingReportsOutputPath;

			return new ProfilingSessionTestCommand(command, new CollectSqlStatementsProfilingSession(connectionString), outputPath);
		}
	}
}
