using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using Newtonsoft.Json;
using NUnit.Framework;
using Relativity;
using Relativity.Logging;
using Relativity.Transfer;
using Relativity.Transfer.Aspera;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2
{
	[TestFixture]
	public class AsperaStorageDeviceServiceTests
	{
		[Test]
		public void ShouldCacheCredentialsForFileshareTest()
		{
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
			
			const int workspaceId = 1049950;
			var exportSettings = new ExportFile(0)
			{
				Credential = new NetworkCredential("serviceaccount@relativity.com", "Test1234!"),
				CaseInfo = new CaseInfo() {ArtifactID = workspaceId}
			};
			var credentialsService = new FileshareCredentialsService(Log.Logger, exportSettings);

			AsperaCredential result = credentialsService.GetCredentialsForFileshare("\\\\files\\T002\\files\\");

			Assert.IsTrue(credentialsService.CachedCredentials.Count > 0);
			Assert.IsNotNull(result);
		}
	}
}
