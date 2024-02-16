﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionConstants.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines static import/export version constants.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange
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
		/// Please note that a minimum version of Relativity is required to perform Import and Export operations using RDC.
		/// </remarks>
		public static Version MinRelativityVersion { get; } = new Version(12, 3, 857, 3);

		/// <summary>
		/// Gets WebApi version for feature: https://jira.kcura.com/browse/REL-408645, when we started support of processing on the server side
		/// the associated documents for objects being imported.
		/// </summary>
		/// <value>
		/// The <see cref="Version"/> instance.
		/// </value>
		public static Version AssociatedDocsImportServerSideSupportFromWebApiVersion { get; } = new Version(1, 1);

		/// <summary>
		/// Gets minimal Prairie Smoke Relativity version.
		/// </summary>
		/// <value>
		/// The <see cref="Version"/> instance.
		/// </value>
		public static Version PrairieSmokeVersion { get; } = new Version(12, 2);
	}
}