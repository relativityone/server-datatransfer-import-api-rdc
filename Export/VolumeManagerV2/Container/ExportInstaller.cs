using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using kCura.WinEDDS.IO;
using kCura.WinEDDS.Service.Export;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ExportInstaller : IWindsorInstaller
	{
		private const string _EXPORT_SUB_SYSTEM_NAME = "Export";

		private readonly Exporter _exporter;
		private readonly string[] _columnNamesInOrder;

		protected ExportFile ExportSettings => _exporter.Settings;

		public ExportInstaller(Exporter exporter, string[] columnNamesInOrder)
		{
			_exporter = exporter;
			_columnNamesInOrder = columnNamesInOrder;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			InstallFromWinEdds(container);
			InstallConnectionToWinEdds(container);
			InstallCustom(container);
			
			container.Register(Classes.FromThisAssembly().InNamespace("kCura.WinEDDS.Core.Export", true).WithService.DefaultInterfaces().WithService.Self());
		}

		private void InstallFromWinEdds(IWindsorContainer container)
		{
			container.Register(Component.For<PaddingWarningValidator>().ImplementedBy<PaddingWarningValidator>());
			container.Register(Component.For<ILoadFileHeaderFormatterFactory>().ImplementedBy<ExportFileFormatterFactory>());
			container.Register(Component.For<IFileStreamFactory>().ImplementedBy<FileStreamFactory>());
			container.Register(Component.For<ITransferClientHandler, IExportFileDownloaderStatus, ExportFileDownloaderStatus>().ImplementedBy<ExportFileDownloaderStatus>());
			container.Register(Component.For<ILoadFileCellFormatter>().UsingFactoryMethod(k => k.Resolve<LoadFileCellFormatterFactory>().Create(ExportSettings)));
			container.Register(Component.For<ExportStatistics, WinEDDS.Statistics>().Instance(_exporter._statistics));
		}

		private void InstallConnectionToWinEdds(IWindsorContainer container)
		{
			container.Register(Component.For<ExportFile>().Instance(ExportSettings));
			container.Register(Component.For<IExportManager>().Instance(_exporter.ExportManager));
			container.Register(Component.For<IStatus>().Instance(_exporter));
			container.Register(Component.For<IFileNameProvider>().Instance(_exporter.FileNameProvider));
			container.Register(Component.For<IUserNotification>().Instance(_exporter.InteractionManager));
			container.Register(Component.For<IFileHelper>().Instance(_exporter.FileHelper));
			container.Register(Component.For<IDirectoryHelper>().Instance(_exporter.DirectoryHelper));
		}

		private void InstallCustom(IWindsorContainer container)
		{
			InstallFieldService(container);
			InstallDirectory(container);
			InstallNatives(container);
			InstallImages(container);
			InstallLongText(container);
			InstallStatefulComponents(container);
			InstallStatistics(container);

			// OTHER
			container.Register(Component.For<IErrorFile>().UsingFactoryMethod(k => k.Resolve<ErrorFileDestinationPath>()));
			container.Register(Component.For<IFilePathTransformer>().UsingFactoryMethod(k => k.Resolve<FilePathTransformerFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IBatchValidator>().UsingFactoryMethod(k => k.Resolve<BatchValidatorFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IBatchInitialization>().UsingFactoryMethod(k => k.Resolve<BatchInitializationFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<ILog>().UsingFactoryMethod(k => RelativityLogFactory.CreateLog(_EXPORT_SUB_SYSTEM_NAME)));
		}

		private void InstallFieldService(IWindsorContainer container)
		{
			container.Register(Component.For<ILoadFileHeaderFormatter>().UsingFactoryMethod(k => k.Resolve<ILoadFileHeaderFormatterFactory>().Create(ExportSettings)));
			container.Register(Component.For<IFieldLookupService, IFieldService, FieldService>()
				.UsingFactoryMethod(k => k.Resolve<FieldServiceFactory>().Create(ExportSettings, _exporter.Columns, _columnNamesInOrder)));
		}

		private void InstallDirectory(IWindsorContainer container)
		{
			container.Register(Component.For<IVolume, Directories.VolumeManager>().ImplementedBy<Directories.VolumeManager>());
			container.Register(Component.For<ISubdirectoryManager, ISubdirectory, SubdirectoryManager>().ImplementedBy<SubdirectoryManager>());
		}

		private void InstallNatives(IWindsorContainer container)
		{
			container.Register(Component.For<IRepository, NativeRepository>().ImplementedBy<NativeRepository>());
		}

		private void InstallImages(IWindsorContainer container)
		{
			container.Register(Component.For<IImagesRollup>().UsingFactoryMethod(k => k.Resolve<ImagesRollupFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileMetadataBuilder>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileMetadataBuilderFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileEntryFactory>().Create(ExportSettings, container)));

			container.Register(Component.For<IRepository, ImageRepository>().ImplementedBy<ImageRepository>());
			container.Register(Component.For<IImageLoadFile>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileFactory>().Create(ExportSettings, container)));
		}

		private void InstallLongText(IWindsorContainer container)
		{
			container.Register(Component.For<IRepository, LongTextRepository>().ImplementedBy<LongTextRepository>());
			container.Register(Component.For<IFullTextLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<FullTextLoadFileEntryFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<ILongTextHandler>().UsingFactoryMethod(k => k.Resolve<LongTextHandlerFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IDelimiter>().UsingFactoryMethod(k => k.Resolve<DelimiterFactory>().Create(ExportSettings)));
		}

		private void InstallStatefulComponents(IWindsorContainer container)
		{
			container.Register(Component.For<IStateful, ILoadFileWriter>().ImplementedBy<LoadFileWriterRetryable>(),
				Component.For<IStateful>().UsingFactoryMethod(k => k.Resolve<IImageLoadFileWriter>()));
		}

		private void InstallStatistics(IWindsorContainer container)
		{
			container.Register(Component.For<IStateful, IFileProcessingStatistics, FilesStatistics>().ImplementedBy<FilesStatistics>());
			container.Register(Component.For<IStateful, IMetadataProcessingStatistics, MetadataStatistics>().ImplementedBy<MetadataStatistics>());
			container.Register(Component.For<IStateful, IDownloadProgress, DownloadProgressManager>().ImplementedBy<DownloadProgressManager>());
		}
	}
}