namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Linq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

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
			return _longTextRepository.GetExportRequests().ToList();
        }

        public List<ExportRequest> RetrieveFileExportRequests()
        {
            return _nativeRepository.GetExportRequests().Concat(_imageRepository.GetExportRequests()).ToList();
        }
    }
}