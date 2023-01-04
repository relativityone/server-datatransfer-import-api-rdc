// <copyright file="IgnoreIfVersionLowerThanAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;
	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using Relativity.DataExchange.TestFramework.RelativityVersions;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
	public sealed class IgnoreIfVersionLowerThanAttribute : Attribute, ITestAction
	{
		public IgnoreIfVersionLowerThanAttribute(RelativityVersion version)
	    {
		    this.Version = version;
	    }

		public RelativityVersion Version { get; }

		public ActionTargets Targets { get; }

		public void BeforeTest(ITest test)
		{
			RelativityVersionChecker.SkipTestIfRelativityVersionIsLowerThan(IntegrationTestHelper.IntegrationTestParameters, this.Version);
		}

		public void AfterTest(ITest test)
		{
		}
    }
}
