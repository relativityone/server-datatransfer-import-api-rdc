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
	using System.Threading.Tasks;

	using Relativity.Logging;

	/// <summary>
	/// This class represents version compatibility validation methods between IAPI client and Relativity/WebAPi.
	/// </summary>
	internal class ImportExportCompatibilityCheck
	{
		private readonly ILog log;

		private IApplicationVersionService applicationVersionService;

		/// <summary>
		/// Initializes a new instance of the <see cref="ImportExportCompatibilityCheck"/> class.
		/// </summary>
		/// /// <param name="applicationVersionService">
		/// Relativity version service object.
		/// </param>
		/// <param name="log">
		/// the log object.
		/// </param>
		public ImportExportCompatibilityCheck(IApplicationVersionService applicationVersionService, ILog log)
		{
			this.log = log.ThrowIfNull(nameof(log));
			this.applicationVersionService = applicationVersionService.ThrowIfNull(nameof(applicationVersionService));
		}

		/// <summary>
		/// This method checks compatibility between IAPI client represented by <see cref="ImportExportApiClientVersion"/> and Relativity/WebApi service.
		/// </summary>
		/// <returns>true if IAPI client compatible with WebAPI service.</returns>
		public bool ValidateCompatibility()
		{
			this.log.LogDebug("Retrieving Relativity version from WebApi");

			Version relativityVersion = this.applicationVersionService.RetrieveRelativityVersion().ConfigureAwait(false).GetAwaiter().GetResult();

			this.log.LogInformation($"Connected to Relativity {relativityVersion} version");

			if (VerifySemanticVersionCheckAvailable(relativityVersion))
			{
				this.log.LogDebug("Trying to get ImportExportWebApiVersion setting");

				// Now we know we can call WebApi to grab it's version
				Version importExportWebApiVersion = this.applicationVersionService.RetrieveImportExportWebApiVersion().ConfigureAwait(false).GetAwaiter().GetResult();

				this.log.LogInformation($"Connected to WebApi {importExportWebApiVersion} version");

				this.log.LogDebug("Trying to perform semantic version compatibility check with WebApi");

				return CheckSemanticVersionsCompability(ImportExportApiClientVersion.Version, importExportWebApiVersion);
			}

			this.log.LogDebug("Trying to get perform compatibility version check with Relativity");
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
