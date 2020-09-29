// <copyright file="IgnoreIfRegressionEnvironmentAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.Interfaces;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
	public sealed class IgnoreIfRegressionEnvironmentAttribute : Attribute, ITestAction
	{
		public IgnoreIfRegressionEnvironmentAttribute(string reasonOfIgnoreTests)
		{
			this.ReasonOfIgnoreTests = reasonOfIgnoreTests;
		}

		public ActionTargets Targets { get; }

		public string ReasonOfIgnoreTests { get; }

		public void BeforeTest(ITest test)
		{
			if (IntegrationTestHelper.IsRegressionEnvironment())
			{
				Assert.Ignore($"Test is set to be ignored on regression environments. {this.ReasonOfIgnoreTests}");
			}
		}

		public void AfterTest(ITest test)
		{
		}
	}
}