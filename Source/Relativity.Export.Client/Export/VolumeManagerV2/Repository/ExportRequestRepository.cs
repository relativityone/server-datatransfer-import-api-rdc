namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class ExportRequestRepository : IExportRequestRepository
	{
		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;

		public ExportRequestRepository(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			return
				_nativeRepository.AnyRequestForLocation(destinationLocation) ||
				_imageRepository.AnyRequestForLocation(destinationLocation) ||
				_longTextRepository.AnyRequestForLocation(destinationLocation);
		}
	
	}
}