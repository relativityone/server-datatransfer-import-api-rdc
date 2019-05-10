namespace Relativity.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;

	using Relativity.Export.VolumeManagerV2.Repository;

	public class ExportRequestRetriever : IExportRequestRetriever
    {
        private readonly NativeRepository _nativeRepository;
        private readonly ImageRepository _imageRepository;
        private readonly LongTextRepository _longTextRepository;

        public ExportRequestRetriever(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository)
        {
            _nativeRepository = nativeRepository;
            _imageRepository = imageRepository;
            _longTextRepository = longTextRepository;
        }

        public List<LongTextExportRequest> RetrieveLongTextExportRequests()
        {
	        IEnumerable<LongTextExportRequest> longTextExportRequests = _longTextRepository.GetExportRequests();
            return new List<LongTextExportRequest>(longTextExportRequests);
        }

        public List<ExportRequest> RetrieveFileExportRequests()
        {
            var fileExportRequests = new List<ExportRequest>();

	        IEnumerable<ExportRequest> nativeExportRequests = _nativeRepository.GetExportRequests();
            fileExportRequests.AddRange(nativeExportRequests);

	        IEnumerable<ExportRequest> imageExportRequests = _imageRepository.GetExportRequests();
            fileExportRequests.AddRange(imageExportRequests);
            return fileExportRequests;
        }
    }
}