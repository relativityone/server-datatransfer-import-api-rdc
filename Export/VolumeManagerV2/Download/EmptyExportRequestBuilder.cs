using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class EmptyExportRequestBuilder : IFileExportRequestBuilder
	{
		public IList<FileExportRequest> Create(ObjectExportInfo artifact)
		{
			return Enumerable.Empty<FileExportRequest>().ToList();
		}
	}
}