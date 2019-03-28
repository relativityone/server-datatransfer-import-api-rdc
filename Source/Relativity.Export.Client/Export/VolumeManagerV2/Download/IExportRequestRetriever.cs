using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
    public interface IExportRequestRetriever
    {
        List<LongTextExportRequest> RetrieveLongTextExportRequests();
        List<ExportRequest> RetrieveFileExportRequests();
    }
}