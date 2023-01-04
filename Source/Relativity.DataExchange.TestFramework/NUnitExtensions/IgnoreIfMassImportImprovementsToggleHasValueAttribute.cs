// <copyright file="IgnoreIfMassImportImprovementsToggleHasValueAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;

	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	/// <summary>
	/// This attribute throws an exception when tests are executed on a hopper with Relativity Lanceleaf.
	/// Therefore, for the tests which we are executing on hoppers, we should always define that attribute
	/// before IgnoreIfVersionLowerThan or IgnoreIfVersionGreaterOrEqual attributes on the attributes list.
	/// </summary>
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
