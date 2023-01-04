// <copyright file="AutomatedPerformanceTestAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System;

	/// <summary>
	/// The tests tagged with this attribute should be run in performance pipeline.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class AutomatedPerformanceTestAttribute : Attribute
	{
	}
}
