// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportCompatibilityCheck.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   This class represents version compatibility validation methods between IAPI client and Relativity/WebAPi.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.Import.Export
{
	using System;

	using Relativity.Logging;

	/// <summary>
	/// This class represents version compatibility validation methods between IAPI client and Relativity/WebAPi.
	/// </summary>
	internal class ImportExportCompatibilityCheck
	{
		private readonly ILog log;

		private IRelativityVersionService relativityVersionService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportCompatibilityCheck"/> class.
		/// </summary>
		/// /// <param name="relativityVersionService">
		/// Relativity version service object.
		/// </param>
		/// <param name="log">
		/// the log object.
		/// </param>
		public ImportExportCompatibilityCheck(IRelativityVersionService relativityVersionService, ILog log)
		{
			this.log = log.ThrowIfNull(nameof(log));
			this.relativityVersionService = relativityVersionService.ThrowIfNull(nameof(relativityVersionService));
		}

		/// <summary>
		/// This method checks compatibility between IAPI client represented by <see cref="ImportExportApiClientVersion"/> and Relativity/WebApi service.
		/// </summary>
		/// <returns>true if IAPI client compatible with WebAPI service.</returns>
		public bool ValidateCompatibility()
		{
			this.log.LogDebug("Retrieving Relativity version from WebApi");

			Version relativityVersion = this.relativityVersionService.RetrieveRelativityVersion();

			if (VerifySemanticVersionCheckAvailable(relativityVersion))
			{
				this.log.LogDebug("Trying to get ImportExportWebApiVersion setting");

				// Now we know we can call WebApi to grab it's version
				Version importExportWebApiVersion = this.relativityVersionService.RetrieveImportExportWebApiVersion();

				return CheckSemanticVersionsCompability(ImportExportApiClientVersion.Version, importExportWebApiVersion);
			}

			return CheckMinimalCompatibleRelativityVersion(relativityVersion);
		}

		private static bool CheckMinimalCompatibleRelativityVersion(Version relativityVersion)
		{
			return relativityVersion >= MinimalCompatibleRelativityVersion.Version;
		}

		private static bool CheckSemanticVersionsCompability(Version importExportApiClientVersion, Version importExportWebApiVersion)
		{
			// Check if there was any breaking change in contract
			return importExportApiClientVersion.Major == importExportWebApiVersion.Major;
		}

		private static bool VerifySemanticVersionCheckAvailable(Version relativityVersion)
		{
			return relativityVersion >= MinimalCompatibleRelativitySemanticVersion.Version;
		}
	}
}
