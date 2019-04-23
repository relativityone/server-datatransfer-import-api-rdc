// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportApiClientVersion.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents internal IAPI client version.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Represents internal IAPI client version.
	/// </summary>
	internal static class ImportExportApiClientVersion
	{
		private const int Major = 1;

		private const int Minor = 0;

		private const int Patch = 0;

		/// <summary>
		/// Gets actual version of IAPI client package compatible with Semantic Versioning :https://semver.org/.
		/// </summary>
		public static Version Version { get; } = new Version(Major, Minor, Patch);
	}
}
