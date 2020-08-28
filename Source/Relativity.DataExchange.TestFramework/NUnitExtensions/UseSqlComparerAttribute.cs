// <copyright file="UseSqlComparerAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal.Commands;

	using Relativity.DataExchange.TestFramework.SqlDataComparer;

	/// <summary>
	/// This attribute is used to mark tests which can be compared using SqlComparer tool.
	/// Comparer config, should be present in "Resources\ClassName\MethodName.xml" file.
	/// Always use this attribute as the last attribute of the test.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class UseSqlComparerAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		public TestCommand Wrap(TestCommand command)
		{
			// It is used to execute tests having this attribute for both MassImportImprovementsToggle values
			// To be sure that repetitions work correct [UseSqlComparer] has to be added as the last test attribute
			// (otherwise operations for another attributes will not be done in each repetition separately)
			const int NumberOfRepetitions = 2;

			return IntegrationTestHelper.IntegrationTestParameters.SqlComparerEnabled
				? new RepeatAttribute(NumberOfRepetitions).Wrap(new SqlComparerTestCommand(command, SqlComparerInputCollector.Instance))
				: command;
		}
	}
}
