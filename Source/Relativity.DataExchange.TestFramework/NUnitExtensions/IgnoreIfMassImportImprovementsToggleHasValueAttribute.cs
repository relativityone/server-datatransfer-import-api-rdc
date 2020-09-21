// <copyright file="IgnoreIfMassImportImprovementsToggleHasValueAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
	public sealed class IgnoreIfMassImportImprovementsToggleHasValueAttribute : Attribute, ITestAction
	{
		public IgnoreIfMassImportImprovementsToggleHasValueAttribute(bool isEnabled)
		{
			this.IsEnabled = isEnabled;
		}

		public bool IsEnabled { get; }

		public ActionTargets Targets { get; }

		public void BeforeTest(ITest test)
		{
			MassImportImprovementsToggleHelper.SkipTestIfMassImportImprovementsToggleHasValue(IntegrationTestHelper.IntegrationTestParameters, this.IsEnabled);
		}

		public void AfterTest(ITest test)
		{
		}
	}
}
