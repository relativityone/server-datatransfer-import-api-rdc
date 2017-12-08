using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class NativeRepositoryBuilderFactory
	{
		private readonly ILog _logger;

		public NativeRepositoryBuilderFactory(ILog logger)
		{
			_logger = logger;
		}

		public NativeRepositoryBuilder Create(ExportFile exportSettings, IWindsorContainer container)
		{
			IFileExportRequestBuilder nativeExportRequestBuilder;

			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(FieldFileExportRequestBuilder));
				nativeExportRequestBuilder = container.Resolve<FieldFileExportRequestBuilder>();
			}
			else
			{
				if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
				{
					_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(NativeFileExportRequestBuilder));
					nativeExportRequestBuilder = container.Resolve<NativeFileExportRequestBuilder>();
				}
				else
				{
					_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(EmptyExportRequestBuilder));
					nativeExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
				}
			}

			return new NativeRepositoryBuilder(container.Resolve<NativeRepository>(), nativeExportRequestBuilder);
		}
	}
}