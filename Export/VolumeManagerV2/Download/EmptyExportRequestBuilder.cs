using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class EmptyExportRequestBuilder : IExportRequestBuilder
	{
		public IList<ExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			return Enumerable.Empty<ExportRequest>().ToList();
		}
	}
}