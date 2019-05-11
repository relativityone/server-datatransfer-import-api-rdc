namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;

	public interface IExportRequestRetriever
    {
        List<LongTextExportRequest> RetrieveLongTextExportRequests();
        List<ExportRequest> RetrieveFileExportRequests();
    }
}