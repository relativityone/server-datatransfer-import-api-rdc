using System.Collections.Generic;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers
{
	public class ExportRequestsWithFileshareSettings
	{
		public RelativityFileShareSettings FileshareSettings { get; }
		public IEnumerable<ExportRequest> Requests { get; }

		public ExportRequestsWithFileshareSettings(RelativityFileShareSettings fileshareSettings, IEnumerable<ExportRequest> requests)
		{
			FileshareSettings = fileshareSettings;
			Requests = requests;
		}
	}
}