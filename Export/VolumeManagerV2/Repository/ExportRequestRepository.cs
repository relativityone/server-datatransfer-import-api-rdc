using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

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

		public IList<ExportRequest> GetExportRequests()
		{
			return _nativeRepository.GetExportRequests().Concat(_imageRepository.GetExportRequests()).Concat(_longTextRepository.GetExportRequests()).ToList();
		}
	}
}