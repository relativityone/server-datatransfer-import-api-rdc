using System.Collections.Generic;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportRequestsWithCredentials
	{
		public AsperaCredential Credentials { get; }
		public IEnumerable<ExportRequest> Requests { get; }

		public ExportRequestsWithCredentials(AsperaCredential credentials, IEnumerable<ExportRequest> requests)
		{
			Credentials = credentials;
			Requests = requests;
		}
	}
}