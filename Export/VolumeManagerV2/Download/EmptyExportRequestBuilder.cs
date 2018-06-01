using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class EmptyExportRequestBuilder : IFileExportRequestBuilder
	{
		public IList<FileExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			return Enumerable.Empty<FileExportRequest>().ToList();
		}
	}
}