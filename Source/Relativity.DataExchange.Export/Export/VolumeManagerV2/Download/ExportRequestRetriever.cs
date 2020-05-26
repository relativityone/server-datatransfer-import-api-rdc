namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

	public class ExportRequestRetriever : IExportRequestRetriever
    {
        private readonly FileRequestRepository _nativeRepository;
        private readonly FileRequestRepository _pdfRepository;
        private readonly ImageRepository _imageRepository;
        private readonly LongTextRepository _longTextRepository;

        public ExportRequestRetriever(FileRequestRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, FileRequestRepository pdfRepository)
        {
            _nativeRepository = nativeRepository;
            _imageRepository = imageRepository;
            _longTextRepository = longTextRepository;
            _pdfRepository = pdfRepository;
        }

        public List<LongTextExportRequest> RetrieveLongTextExportRequests()
        {
			return _longTextRepository.GetExportRequests().ToList();
        }

        public List<ExportRequest> RetrieveFileExportRequests()
        {
            return _nativeRepository.GetExportRequests().Concat(_imageRepository.GetExportRequests()).Concat(_pdfRepository.GetExportRequests()).ToList();
        }
    }
}