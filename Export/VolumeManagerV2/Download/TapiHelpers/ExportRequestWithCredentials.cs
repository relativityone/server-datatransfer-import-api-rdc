using System.Collections.Generic;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportRequestWithCredentials
	{
		public Credential Credentials { get; }
		public IEnumerable<ExportRequest> Requests { get; }

		public ExportRequestWithCredentials(Credential credentials, IEnumerable<ExportRequest> requests)
		{
			Credentials = credentials;
			Requests = requests;
		}
	}
}