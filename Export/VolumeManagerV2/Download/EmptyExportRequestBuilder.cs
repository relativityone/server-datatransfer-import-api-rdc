using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class EmptyExportRequestBuilder : IFileExportRequestBuilder
	{
		public IEnumerable<FileExportRequest> Create(ObjectExportInfo artifact)
		{
			return Enumerable.Empty<FileExportRequest>();
		}
	}
}