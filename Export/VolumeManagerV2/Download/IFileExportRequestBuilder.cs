using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IFileExportRequestBuilder
	{
		IEnumerable<FileExportRequest> Create(ObjectExportInfo artifact);
	}
}