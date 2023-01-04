// <copyright file="PerformanceAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using NUnit.Framework.Internal.Commands;

	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class PerformanceAttribute : PropertyAttribute, IWrapSetUpTearDown
	{
		public TestCommand Wrap(TestCommand command)
		{
			return new PerformanceTestCommand(command);
		}
	}
}
