using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IExportRequestBuilder
	{
		IList<ExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken);
	}
}