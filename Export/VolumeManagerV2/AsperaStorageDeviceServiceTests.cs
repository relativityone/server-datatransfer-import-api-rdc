using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using Newtonsoft.Json;
using NUnit.Framework;
using Relativity.Transfer;
using Relativity.Transfer.Aspera;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class AsperaStorageDeviceServiceTests
	{
		[Test]
		public async Task Connect()
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
				using (ITransferLog transferLog = new SerilogTransferLog())
				using (var transferHost = new RelativityTransferHost(connectionInfo, transferLog))
				{
					// The storage search API is obtained through the transfer host.
					IFileStorageSearch service = transferHost.CreateFileStorageSearch();

					// Note: until the Kepler API changes have been made, you must be an admin and supply the resource pool.
					FileStorageSearchResults results = await service.GetWorkspaceFileSharesAsync(WorkspaceId);
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

		//[Test]
		public void CreateUri()
		{
			AsperaCredentialsServiceMock results = new AsperaCredentialsServiceMock();

			var uri = new Uri("\\\\files1.il1ddftasmfs001.kcura.corp\\T002\\files\\workspaceId\\dupa.txt");
			

			var tapiUri = new Uri("\\\\files1.il1ddftasmfs001.kcura.corp\\T002\\files\\");
			
			Assert.IsTrue(tapiUri.IsBaseOf(uri));
		}
	}

	internal class SerilogTransferLog : ITransferLog
	{
		public void Dispose()
		{
		}

		public void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogDebug(string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogInformation(string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogWarning(string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogError(string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogFatal(string messageTemplate, params object[] propertyValues)
		{
		}

		public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
		}

		public bool IsEnabled { get; set; }
	}
}
