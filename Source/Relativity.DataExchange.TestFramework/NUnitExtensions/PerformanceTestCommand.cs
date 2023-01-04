// <copyright file="PerformanceTestCommand.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
    using NUnit.Framework.Internal;
    using NUnit.Framework.Internal.Commands;
    using Relativity.DataExchange.TestFramework.PerformanceTests;

    internal class PerformanceTestCommand : BeforeAndAfterTestCommand
	{
		public PerformanceTestCommand(TestCommand innerCommand)
			: base(innerCommand)
		{
			this.BeforeTest = ExecuteBeforeTest;
			this.AfterTest = ExecuteAfterTest;
		}

		private void ExecuteBeforeTest(TestExecutionContext context)
		{
			PerformanceDataCollector.SetUpPerformanceLogger();
		}

		private void ExecuteAfterTest(TestExecutionContext context)
		{
			PerformanceDataCollector.Instance.StorePerformanceResults();
		}
	}
}
