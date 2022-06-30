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
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.Kepler.Transport;
	using Relativity.Services.FileSystem;

	public static class BcpFileHelper
	{
		/// <summary>
		/// Creates file in IntegrationTestParameters.BcpSharePath with GUID as a file name.
		/// </summary>
		/// <param name="parameters">Parameters with credentials.</param>
		/// <param name="content">File content.</param>
		/// <param name="destinationPath">Destination path on the server.</param>
		/// <returns>File name.</returns>
		public static async Task<string> CreateAsync(IntegrationTestParameters parameters, string content, string destinationPath)
		{
			UnicodeEncoding encoding = new UnicodeEncoding(false, true);
			byte[] contentBytes = encoding.GetPreamble().Concat(encoding.GetBytes(content)).ToArray();

			string fileName = Guid.NewGuid().ToString();
			using (var fileIOService = ServiceHelper.GetServiceProxy<IFileIOService>(parameters))
			{
				await fileIOService.BeginFillAsync(
					parameters.WorkspaceId,
					contentBytes.Concat(new byte[] { 0x49 }).ToArray(),
					destinationPath + "\\",
					fileName,
					Guid.NewGuid().ToString())
					.ConfigureAwait(false);
			}

			return fileName;
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