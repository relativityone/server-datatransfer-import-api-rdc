// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinimalCompatibleRelativitySemanticVersion.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents Relativity version from which WebApi supports Semantic Versioning
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export.Versioning
{
	using System;

	/// <summary>
	/// Represents internal IAPI client version.
	/// </summary>
	internal static class MinimalCompatibleRelativitySemanticVersion
	{
		private const int Major = 10;

		private const int Minor = 3;

		/// <summary>
		/// Gets Relativity version from which WebApi supports Semantic Versioning.
		/// </summary>
		public static Version Version { get; } = new Version(Major, Minor);
	}
}
