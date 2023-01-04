// <copyright file="IgnoreIfVersionGreaterOrEqualAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using Relativity.DataExchange.TestFramework.RelativityVersions;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
	public sealed class IgnoreIfVersionGreaterOrEqualAttribute : Attribute, ITestAction
	{
		public IgnoreIfVersionGreaterOrEqualAttribute(RelativityVersion version)
		{
			this.Version = version;
		}

		public RelativityVersion Version { get; }

		public ActionTargets Targets { get; }

		public void BeforeTest(ITest test)
		{
			RelativityVersionChecker.SkipTestIfRelativityVersionIsGreaterOrEqual(IntegrationTestHelper.IntegrationTestParameters, this.Version);
		}

		public void AfterTest(ITest test)
		{
		}
	}
}
