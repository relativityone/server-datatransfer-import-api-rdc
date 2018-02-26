using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using NUnit.Framework;
using Relativity.Transfer;
using Relativity.Transfer.Aspera;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class AsperaStorageDeviceServiceTests
	{
		[Test]
		public void Connect()
		{
			try
			{
				// Note: you MUST use an environment that supports the file share/Aspera naming conventions.
				//       The Hyper-V private cloud has already been fully updated.
				var connectionInfo = new RelativityConnectionInfo(
					new Uri("https://il1ddtapirap001.kcura.corp"),
					new BasicAuthenticationCredential("serviceaccount@relativity.com", "Test1234!"));
				IAsperaStorageDeviceService service = new AsperaStorageDeviceService(connectionInfo);

				// Note: until the Kepler API changes have been made, you must be an admin and supply the resource pool.
				var workspaceResults = service.GetStorageDevicesAsync(1017836);


				var poolResults = workspaceResults.ConfigureAwait(false).GetAwaiter().GetResult();


				foreach (AsperaStorageDevice device in poolResults.InvalidStorageDevices)
				{
					Console.WriteLine($"The storage device {device.Name} is invalid");
				}

				foreach (AsperaStorageDevice device in poolResults.StorageDevices)
				{
					Console.WriteLine($"Storage device connection string: {device.ConnectionString}");
					Console.WriteLine($"Storage device doc root: {device.DocRoot}");
					Console.WriteLine($"Storage device name: {device.Name}");
					Console.WriteLine($"Storage device Transfer credential: {device.TransferCredential.Name}");
					Console.WriteLine($"Storage device Node credential: {device.NodeCredential.Name}");
				}
			}
			catch (Exception e)
			{
				// Handle the exception.
			}
		}

		[Test]
		public void CreateUri()
		{
			AsperaCredentialsServiceMock results = new AsperaCredentialsServiceMock();

			var uri = new Uri("\\\\files1.il1ddftasmfs001.kcura.corp\\T002\\files\\workspaceId\\dupa.txt");
			

			var tapiUri = new Uri("\\\\files1.il1ddftasmfs001.kcura.corp\\T002\\files\\");
			
			Assert.IsTrue(tapiUri.IsBaseOf(uri));
		}
	}
}
