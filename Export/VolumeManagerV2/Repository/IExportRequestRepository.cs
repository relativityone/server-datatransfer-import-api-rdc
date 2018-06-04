using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public interface IExportRequestRepository
	{
		IList<ExportRequest> GetExportRequests();
	}
}