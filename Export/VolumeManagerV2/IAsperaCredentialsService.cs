using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public interface IFileshareCredentialsService
	{
		Credential GetCredentialsForFileshare(Uri fileUri);
	}

	public class FileshareCredentialService : IFileshareCredentialsService
	{
		private readonly ITransferLog _logger;

		public FileshareCredentialService(ITransferLog logger)
		{
			_logger = logger;
		}

		public Credential GetCredentialsForFileshare(Uri fileUri)
		{
			try
			{
				// Note: you MUST use an environment that supports the file share/Aspera naming conventions.
				//       The Hyper-V private cloud has already been fully updated.
				const int WorkspaceId = 1049950;
				var connectionInfo = new RelativityConnectionInfo(
					new Uri("https://il1ddtapirap001.kcura.corp"),
					new BasicAuthenticationCredential("serviceaccount@relativity.com", "Test1234!"),
					WorkspaceId);
				using (RelativityTransferHost transferHost = new RelativityTransferHost(connectionInfo, _logger))
				{
					// The storage search API is obtained through the transfer host.
					IFileStorageSearch service = transferHost.CreateFileStorageSearch();

					// Note: until the Kepler API changes have been made, you must be an admin and supply the resource pool.
					FileStorageSearchResults results = await service.GetResourcePoolFileSharesAsync("RelativityOne Pool");
					var str = JsonConvert.SerializeObject(results);
					foreach (RelativityFileShare fileShare in results.InvalidFileShares)
					{
						Console.WriteLine($"The file share {fileShare.Name} is invalid");
					}

					foreach (RelativityFileShare fileShare in results.FileShares)
					{
						Console.WriteLine($"File share URL: {fileShare.Url}");
						Console.WriteLine($"File share doc root: {fileShare.DocRoot}");
						Console.WriteLine($"File share name: {fileShare.Name}");
						Console.WriteLine($"File share number: {fileShare.Number}");
						Console.WriteLine($"File share transfer credential: {fileShare.TransferCredential.Name}");
						Console.WriteLine($"File share node credential: {fileShare.NodeCredential.Name}");

						var result = JsonConvert.SerializeObject(fileShare);
					}
				}
			}
			catch (Exception e)
			{
				// Handle the exception.
			}
		}
	}
}
