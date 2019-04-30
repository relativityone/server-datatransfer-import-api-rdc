﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionConstants.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents Relativity version from which WebApi supports Semantic Versioning
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Defines static import/export version constants.
	/// </summary>
	internal static class VersionConstants
	{
		/// <summary>
		/// Gets the required WebAPI version.
		/// </summary>
		/// <value>
		/// The <see cref="Version"/> instance.
		/// </value>
		public static Version RequiredWebApiVersion { get; } = new Version(1, 0);

		/// <summary>
		/// Gets the minimum Relativity version when the instance doesn't support the new WebAPI version endpoint.
		/// </summary>
		/// <value>
		/// The <see cref="Version"/> instance.
		/// </value>
		/// <remarks>
		/// TODO: Must validate that the Goatsbeard Import API release can support all versions from Bluestem through Goatsbeard.
		/// </remarks>
		public static Version MinRelativityVersion { get; } = new Version(9, 7);

		/// <summary>
		/// Gets the Relativity version from which it supports the new WebAPI version endpoint.
		/// </summary>
		/// <value>
		/// The <see cref="Version"/> instance.
		/// </value>
		public static Version WebApiStartFromRelativityVersion { get; } = new Version(10, 3);
	}
}