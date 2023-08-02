// ----------------------------------------------------------------------------
// <copyright file="ApplicationHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Relativity.Kepler.Transport;
	using Relativity.Services.Interfaces.LibraryApplication;
	using Relativity.Services.Interfaces.LibraryApplication.Models;

	public static class ApplicationHelper
	{
		private static string productionVersion = null;
		private static Guid? productionPackageGuid = null;
		private static Guid? productionApplicationGuid = null;

		public static void InstallProduction(IntegrationTestParameters testParameters)
		{
			if (testParameters == null)
			{
				throw new ArgumentNullException(nameof(testParameters));
			}

			var libraryApplicationManager = ServiceHelper.GetServiceProxy<Relativity.Services.Interfaces.LibraryApplication.ILibraryApplicationManager>(testParameters);
			if (productionPackageGuid.HasValue && productionApplicationGuid.HasValue && !productionVersion.IsNullOrEmpty())
			{
				LibraryApplicationResponse libraryApplicationResponse = libraryApplicationManager
					.ReadAllAsync(-1).GetAwaiter().GetResult().FirstOrDefault(x => x.Guids.FirstOrDefault() == productionApplicationGuid.Value);

				if (libraryApplicationResponse != null && !IsNewerVersion(productionVersion, libraryApplicationResponse.Version))
				{
					return;
				}
			}

			const string pathToRAPFolder = @"\\bld-pkgs\Packages\Productions\master";
			string pathToProductionsRap = GetPathToLatestRAPFrom(pathToRAPFolder);
			var libraryApplicationService = ServiceHelper.GetServiceProxy<IApplicationInstallManager>(testParameters);
			if (!string.IsNullOrEmpty(pathToProductionsRap))
			{
				try
				{
					Console.WriteLine($"Try install package in: {pathToProductionsRap}");
					PackageDetailsResponse packageDetailsResponse;
					using (System.IO.Stream stream = System.IO.File.OpenRead(pathToProductionsRap))
					{
						packageDetailsResponse = libraryApplicationManager.UploadPackageAsync(-1, new KeplerStream(stream)).GetAwaiter().GetResult();
					}

					LibraryApplicationResponse libraryApplicationResponse = libraryApplicationManager.ReadAllAsync(-1).GetAwaiter().GetResult().FirstOrDefault(x => x.Guids.FirstOrDefault() == packageDetailsResponse.ApplicationGUID);

					if (libraryApplicationResponse == null || IsNewerVersion(packageDetailsResponse.Version, libraryApplicationResponse.Version))
					{
						productionApplicationGuid = packageDetailsResponse.ApplicationGUID;
						productionVersion = packageDetailsResponse.Version;
						productionPackageGuid = packageDetailsResponse.PackageGUID;

						UpdateLibraryApplicationResponse updateLibraryApplicationResponse = libraryApplicationManager.UpdateAsync(-1, packageDetailsResponse.PackageGUID, new UpdateLibraryApplicationRequest { CreateIfMissing = true }).GetAwaiter().GetResult();

						InstallApplicationAllRequest request = new InstallApplicationAllRequest
						{
							Mode = ApplicationInstallTargetOption.ForceInstall,
							UnlockApplications = false,
						};
						var installationApplicationResponse = libraryApplicationService.InstallApplicationAllAsync(-1, packageDetailsResponse.ApplicationGUID, request).GetAwaiter().GetResult();
						WaitForInstallationCompleted(testParameters.WorkspaceId, libraryApplicationService);
					}
				}
				catch (Exception ex)
				{
					string exception = $"An error occurred: {ex.Message}";
					Console.WriteLine(exception);
					throw;
				}
			}
		}

		private static void WaitForInstallationCompleted(int workspaceId, IApplicationInstallManager libraryApplicationService)
		{
			if (!productionApplicationGuid.HasValue)
			{
				return;
			}

			var installationStatusResponse = libraryApplicationService.GetStatusAsync(workspaceId, productionApplicationGuid.Value).GetAwaiter().GetResult();
			List<InstallStatusCode> terminalStates = new List<InstallStatusCode>() { InstallStatusCode.Canceled, InstallStatusCode.Completed, InstallStatusCode.Failed };
			InstallStatusCode installationStatus = installationStatusResponse.InstallStatus.Code;
			while (!terminalStates.Contains(installationStatus))
			{
				Task.Delay(new TimeSpan(0, 0, 0, 1)).GetAwaiter().GetResult();
				installationStatusResponse = libraryApplicationService.GetStatusAsync(workspaceId, productionApplicationGuid.Value).GetAwaiter().GetResult();
				installationStatus = installationStatusResponse.InstallStatus.Code;
			}
		}

		private static string GetPathToLatestRAPFrom(string path)
		{
			const string rapName = "Relativity.Productions.rap";

			var dirInfo = new DirectoryInfo(path);
			if (dirInfo.Exists)
			{
				DirectoryInfo directoryInfo = (from f in dirInfo.GetDirectories() orderby f.LastWriteTime descending select f).FirstOrDefault();
				if (directoryInfo != null)
				{
					FileInfo file = (from f in directoryInfo.GetFiles(rapName) select f).FirstOrDefault();
					return file?.FullName;
				}
			}

			return null;
		}

		private static bool IsNewerVersion(string uploadedApplicationVersion, string existingApplicationVersion)
		{
			var packageVersion = new Version(uploadedApplicationVersion);
			var applicationVersion = new Version(existingApplicationVersion);

			if (packageVersion.CompareTo(applicationVersion) > 0)
			{
				return true;
			}

			return false;
		}
	}
}