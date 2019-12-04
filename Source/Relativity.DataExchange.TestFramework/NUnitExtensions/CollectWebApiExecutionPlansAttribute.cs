// <copyright file="CollectWebApiExecutionPlansAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal.Commands;

	using Relativity.DataExchange.TestFramework.WebApiSqlProfiling.ExecutionPlan;

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CollectWebApiExecutionPlansAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		private readonly string connectionString;

		public CollectWebApiExecutionPlansAttribute()
		{
			this.connectionString = IntegrationTestHelper.GetSqlConnectionStringBuilder().ConnectionString;
		}

		public TestCommand Wrap(TestCommand command)
		{
			return IntegrationTestHelper.ReadIntegrationTestParameters().SqlCaptureProfiling
					   ? new ProfilingSessionTestCommand(command, new CollectExecutionPlansProfilingSession(this.connectionString))
					   : command;
		}
	}
}
