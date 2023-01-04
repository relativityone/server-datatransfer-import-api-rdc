namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using Castle.Windsor;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Container;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.Logging;

	public class NativeRepositoryBuilderFactory
	{
		private readonly ILog _logger;

		public NativeRepositoryBuilderFactory(ILog logger)
		{
			_logger = logger;
		}

		public IRepositoryBuilder Create(ExportFile exportSettings, IWindsorContainer container)
		{
			IExportRequestBuilder nativeExportRequestBuilder;

			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
				{
					_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(FieldFileExportRequestBuilder));
					nativeExportRequestBuilder = container.Resolve<FieldFileExportRequestBuilder>();
				}
				else
				{
					_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(NativeFileExportRequestBuilder));
					nativeExportRequestBuilder = container.Resolve<NativeFileExportRequestBuilder>();
				}
			}
			else
			{
				_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(EmptyExportRequestBuilder));
				nativeExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
			}

			return new FileRepositoryBuilder(
				container.Resolve<FileRequestRepository>(ExportInstaller.GetServiceNameByExportType(typeof(FileRequestRepository), ExportFileTypes.Native)), 
				container.Resolve<ILabelManagerForArtifact>(), nativeExportRequestBuilder, _logger);
		}
	}
}