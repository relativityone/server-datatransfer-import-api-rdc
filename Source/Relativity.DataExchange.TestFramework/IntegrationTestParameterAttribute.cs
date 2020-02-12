﻿// ----------------------------------------------------------------------------
// <copyright file="IntegrationTestParameterAttribute.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;

	/// <summary>
	/// Represents an integration test parameter property-based attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IntegrationTestParameterAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationTestParameterAttribute"/> class.
		/// </summary>
		/// <param name="mapped">
		/// <see langword="true" /> if the test parameter is mapped; otherwise, <see langword="false" />.
		/// </param>
		public IntegrationTestParameterAttribute(bool mapped)
		{
			this.IsMapped = mapped;
		}

		/// <summary>
		/// Gets a value indicating whether the associated parameter is mapped to an App.Config setting.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if the test parameter is mapped; otherwise, <see langword="false" />.
		/// </value>
		public bool IsMapped
		{
			get;
		}
	}
}