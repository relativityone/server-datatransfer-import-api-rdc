// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCompatibilityCheckResult.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   This class represents version compatibility validation methods between IAPI client and Relativity/WebAPi.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// This class represents result of version compatibility check between IApi client and WebApi/Relativity.
	/// </summary>
	public class VersionCompatibilityCheckResult
	{
		/// <summary>
		/// Gets or sets a value indicating whether the result of version compatibility check result between IApi client and Relativity/WebApi. True if IApi client is compatible with WebApi/Relativity.
		/// </summary>
		public bool CompatibilityResult { get; set; }

		/// <summary>
		/// Gets or Sets Relativity instance version.
		/// </summary>
		public Version RelativityVersion { get; set; }

		/// <summary>
		/// Gets or Sets WebApi version. This value is only set when connecting to Relativity that implements WebApi semantic versioning.
		/// </summary>
		public Version WebApiVersion { get; set; }
	}
}
