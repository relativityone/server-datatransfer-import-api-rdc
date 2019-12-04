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
		private readonly string connectionString;

		public CollectWebApiSqlAttribute()
		{
			this.connectionString = IntegrationTestHelper.GetSqlConnectionStringBuilder().ConnectionString;
		}

		public TestCommand Wrap(TestCommand command)
		{
			return IntegrationTestHelper.ReadIntegrationTestParameters().SqlCaptureProfiling
					   ? new ProfilingSessionTestCommand(command, new CollectSqlStatementsProfilingSession(this.connectionString))
					   : command;
		}
	}
}
