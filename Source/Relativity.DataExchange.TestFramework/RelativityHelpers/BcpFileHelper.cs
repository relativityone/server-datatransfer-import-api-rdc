// <copyright file="BcpFileHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Text;
	using System.Threading.Tasks;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Kepler.Transport;
	using Relativity.Services.FileSystem;

	public static class BcpFileHelper
	{
		/// <summary>
		/// Creates file in IntegrationTestParameters.BcpSharePath with GUID as a file name.
		/// </summary>
		/// <param name="parameters">Parameters with credentials.</param>
		/// <param name="content">File content.</param>
		/// <param name="bcpPath">BCP Path for workspace.</param>
		/// <returns>File name.</returns>
		public static async Task<string> CreateAsync(IntegrationTestParameters parameters, string content, string bcpPath)
		{
			if (RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.Indigo))
			{
				throw new Exception(
					"This method uses IFileSystemManager that was added in RelativityVersion.Indigo and current version is older");
			}

			UnicodeEncoding encoding = new UnicodeEncoding(false, true);
			byte[] contentBytes = encoding.GetPreamble().Concat(encoding.GetBytes(content)).ToArray();

			using (var fileSystemManager = ServiceHelper.GetServiceProxy<IFileSystemManager>(parameters))
			using (var stream = new MemoryStream(contentBytes))
			using (var keplerStream = new KeplerStream(stream))
			{
				string fileName = Guid.NewGuid().ToString();

				await fileSystemManager.UploadFileAsync(keplerStream, Path.Combine(bcpPath, fileName)).ConfigureAwait(false);

				return fileName;
			}
		}

		public static Task<string> CreateEmptyAsync(IntegrationTestParameters parameters, string bcpPath)
		{
			return CreateAsync(parameters, string.Empty, bcpPath);
		}

		public static string GetBcpPath(IntegrationTestParameters parameters, NetworkCredential credential, CookieContainer cookieContainer, Func<string> correlationIdFunc)
		{
			if (parameters == null)
			{
				throw new ArgumentException($"{nameof(parameters)} parameter should not be null");
			}

			using (IFileIO fileIo = ManagerFactory.CreateFileIO(
				credential,
				cookieContainer,
				correlationIdFunc))
			{
				return fileIo.GetBcpSharePath(parameters.WorkspaceId);
			}
		}
	}
}