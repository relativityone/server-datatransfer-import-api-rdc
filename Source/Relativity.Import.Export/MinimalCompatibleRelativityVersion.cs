// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MinimalCompatibleRelativityVersion.cs" company="Relativity ODA LLC">
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
	/// In case IAPI connects to the older Relativity/WebApi version that did not implement Semantic Versioning, the contract can be still compatible between
	/// new IAPI client and this Relativity instance. This class represents minimal version of Relativity that can be supported by IAPI client
	/// represented by ImportExportApiClientVersion class.
	/// </summary>
	internal class MinimalCompatibleRelativityVersion
	{
		private const int Major = 9;

		private const int Minor = 7;

		private const int Patch = 229;

		/// <summary>
		/// Gets Blustem3 version that is currently the minimal compatible version of Relativity/WebApi that IAPI client can connect to.
		/// </summary>
		public static Version Version { get; } = new Version(Major, Minor, Patch);
	}
}
