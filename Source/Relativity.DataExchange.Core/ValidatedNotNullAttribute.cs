// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidatedNotNullAttribute.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Extension to check null pointer arguments on the function call
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange
{
	using System;

	/// <summary>
	/// This class informs FxCop that the argument passed in, is checked for null. This means we do not need to disable warnings manually everywhere.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class ValidatedNotNullAttribute : Attribute
	{
	}
}
