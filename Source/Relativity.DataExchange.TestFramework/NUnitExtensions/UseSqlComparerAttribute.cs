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
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class UseSqlComparerAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		public TestCommand Wrap(TestCommand command)
		{
			return IntegrationTestHelper.IntegrationTestParameters.SqlComparerEnabled
				? new SqlComparerTestCommand(command, SqlComparerInputCollector.Instance)
				: command;
		}
	}
}
