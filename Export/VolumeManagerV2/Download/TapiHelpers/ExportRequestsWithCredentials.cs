using System.Collections.Generic;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportRequestsWithCredentials
	{
		public Credential Credentials { get; }
		public IEnumerable<ExportRequest> Requests { get; }

		public ExportRequestsWithCredentials(Credential credentials, IEnumerable<ExportRequest> requests)
		{
			Credentials = credentials;
			Requests = requests;
		}
	}
}