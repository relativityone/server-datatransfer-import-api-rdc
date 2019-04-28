// --------------------------------------------------------------------------------------------------------------------
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
	/// Represents the WebAPI and Relativity version constants.
	/// </summary>
	internal static class VersionConstants
	{
		/// <summary>
		/// Gets the minimum WebAPI version.
		/// </summary>
		public static Version MinWebApiVersion { get; } = new Version(1, 0);

		/// <summary>
		/// Gets the minimum Relativity version when the instance doesn't support the new WebAPI version endpoint.
		/// </summary>
		/// <remarks>
		/// TODO: Must validate that the Goatsbeard Import API release can support all versions from Bluestem through Goatsbeard.
		/// </remarks>
		public static Version MinRelativityVersion { get; } = new Version(9, 7);
	}
}