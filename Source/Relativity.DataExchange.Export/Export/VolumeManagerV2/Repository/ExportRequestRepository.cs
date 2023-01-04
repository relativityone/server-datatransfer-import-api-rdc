namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	public class ExportRequestRepository : IExportRequestRepository
	{
		private readonly FileRequestRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly FileRequestRepository _pdfRepository;

		public ExportRequestRepository(FileRequestRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, FileRequestRepository pdfRepository)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_pdfRepository = pdfRepository;
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			return
				_nativeRepository.AnyRequestForLocation(destinationLocation) ||
				_imageRepository.AnyRequestForLocation(destinationLocation) ||
				_longTextRepository.AnyRequestForLocation(destinationLocation) ||
				this._pdfRepository.AnyRequestForLocation(destinationLocation);
		}
	
	}
}