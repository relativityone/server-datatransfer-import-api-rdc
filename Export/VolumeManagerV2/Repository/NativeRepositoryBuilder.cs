using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class NativeRepositoryBuilder
	{
		private readonly NativeRepository _nativeRepository;
		private readonly IFileExportRequestBuilder _fileExportRequestBuilder;

		public NativeRepositoryBuilder(NativeRepository nativeRepository, IFileExportRequestBuilder fileExportRequestBuilder)
		{
			_nativeRepository = nativeRepository;
			_fileExportRequestBuilder = fileExportRequestBuilder;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			IList<FileExportRequest> exportRequests = _fileExportRequestBuilder.Create(artifact, cancellationToken);

			Native native = new Native(artifact)
			{
				ExportRequest = exportRequests.FirstOrDefault(),
				HasBeenDownloaded = exportRequests.Count == 0
			};

			_nativeRepository.Add(native.InList());
		}
	}
}