﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Container
{
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text.Delimiter;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Settings;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	using Relativity.Logging;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.Exporters.Validator;
	using kCura.WinEDDS.Service.Export;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.Transfer;

	public class ExportInstaller : IWindsorInstaller
	{
		private const string _EXPORT_SUB_SYSTEM_NAME = "Export";

		private readonly Exporter _exporter;
		private readonly string[] _columnNamesInOrder;
		private readonly ILoadFileHeaderFormatterFactory _loadFileHeaderFormatterFactory;

		protected ExportFile ExportSettings => _exporter.Settings;
		protected IExportConfig ExportConfig => _exporter.ExportConfig;

		public ExportInstaller(Exporter exporter, string[] columnNamesInOrder, ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
			_exporter = exporter;
			_columnNamesInOrder = columnNamesInOrder;
			_loadFileHeaderFormatterFactory = loadFileHeaderFormatterFactory;
		}

		public virtual void Install(IWindsorContainer container, IConfigurationStore store)
		{
			InstallFromWinEdds(container);
			InstallConnectionToWinEdds(container);
			InstallCustom(container);

			container.Register(Classes.FromThisAssembly().InNamespace("Relativity.DataExchange.Export", true).WithService.DefaultInterfaces().WithService.Self());
			this.OnInstall(container, store);
		}

		protected virtual void OnInstall(IWindsorContainer container, IConfigurationStore store)
		{
		}

		private void InstallFromWinEdds(IWindsorContainer container)
		{
			container.Register(Component.For<PaddingWarningValidator>().ImplementedBy<PaddingWarningValidator>());
			container.Register(Component.For<IImageConverter>().ImplementedBy<ImageConverterService>());
			container.Register(Component.For<ILoadFileHeaderFormatterFactory>().Instance(_loadFileHeaderFormatterFactory));
			container.Register(Component.For<ITransferClientHandler, IExportFileDownloaderStatus, ExportFileDownloaderStatus>().ImplementedBy<ExportFileDownloaderStatus>());
			container.Register(Component.For<ILoadFileCellFormatter>().UsingFactoryMethod(k => k.Resolve<LoadFileCellFormatterFactory>().Create(ExportSettings, k.Resolve<FilePathTransformerFactory>().Create(ExportSettings, container))));
			container.Register(Component.For<ExportStatistics, kCura.WinEDDS.Statistics>().Instance(_exporter.Statistics));
		}

		private void InstallConnectionToWinEdds(IWindsorContainer container)
		{
			container.Register(Component.For<ExportFile>().Instance(ExportSettings));
			container.Register(Component.For<IExportManager>().Instance(_exporter.ExportManager));
			container.Register(Component.For<IStatus>().Instance(_exporter));
			container.Register(Component.For<IFileNameProvider>().Instance(_exporter.FileNameProvider));
			container.Register(Component.For<IUserNotification>().Instance(_exporter.InteractionManager));
			container.Register(Component.For<IFile>().Instance(_exporter.FileHelper));
			container.Register(Component.For<IDirectory>().Instance(_exporter.DirectoryHelper));
			container.Register(Component.For<IExportConfig>().Instance(_exporter.ExportConfig));
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
			container.Register(Component.For<IBatchValidator>().UsingFactoryMethod(k => k.Resolve<BatchValidatorFactory>().Create(ExportSettings, ExportConfig, container)));
			container.Register(Component.For<IBatchInitialization>().UsingFactoryMethod(k => k.Resolve<BatchInitializationFactory>().Create(ExportSettings, ExportConfig, container)));
			container.Register(Component.For<ILog>().UsingFactoryMethod(k => RelativityLogFactory.CreateLog(_EXPORT_SUB_SYSTEM_NAME)));
			container.Register(Component.For<ITapiObjectService>().ImplementedBy<TapiObjectService>());

			container.Register(Component.For<ILabelManagerForArtifact>().UsingFactoryMethod(k =>
				ExportConfig.ForceParallelismInNewExport
					? (ILabelManagerForArtifact)k.Resolve<CachedLabelManagerForArtifact>()
					: k.Resolve<LabelManagerForArtifact>()));
		}

		private void InstallFieldService(IWindsorContainer container)
		{
			container.Register(Component.For<ILoadFileHeaderFormatter>().UsingFactoryMethod(k => k.Resolve<ILoadFileHeaderFormatterFactory>().Create(ExportSettings)));
			container.Register(Component.For<IFieldLookupService, IFieldService, FieldService>()
				.UsingFactoryMethod(k => k.Resolve<FieldServiceFactory>().Create(ExportSettings, _columnNamesInOrder)));
		}

		private void InstallDirectory(IWindsorContainer container)
		{
			container.Register(Component.For<IVolume, Directories.VolumeManager>().ImplementedBy<Directories.VolumeManager>());
			container.Register(Component.For<ISubdirectoryManager, ISubdirectory, SubdirectoryManager>().ImplementedBy<SubdirectoryManager>());
		}

		private void InstallNatives(IWindsorContainer container)
		{
			container.Register(Component.For<IClearable, NativeRepository>().ImplementedBy<NativeRepository>());
		}

		private void InstallImages(IWindsorContainer container)
		{
			container.Register(Component.For<IImagesRollup>().UsingFactoryMethod(k => k.Resolve<ImagesRollupFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileMetadataBuilder>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileMetadataBuilderFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileEntryFactory>().Create(ExportSettings, container)));

			container.Register(Component.For<IClearable, ImageRepository>().ImplementedBy<ImageRepository>());
			container.Register(Component.For<IImageLoadFile>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileFactory>().Create(ExportSettings, container)));
		}

		private void InstallLongText(IWindsorContainer container)
		{
			container.Register(Component.For<IClearable, ILongTextRepository, LongTextRepository>().ImplementedBy<LongTextRepository>());
			container.Register(Component.For<IFullTextLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<FullTextLoadFileEntryFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<ILongTextHandler>().UsingFactoryMethod(k => k.Resolve<LongTextHandlerFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IDelimiter>().UsingFactoryMethod(k => k.Resolve<DelimiterFactory>().Create(ExportSettings)));
			container.Register(Component.For<ILongTextStreamFormatterFactory>().UsingFactoryMethod(k => k.Resolve<LongTextStreamFormatterFactoryFactory>().Create(ExportSettings)));
		}

		private void InstallStatefulComponents(IWindsorContainer container)
		{
			container.Register(Component.For<IStateful, ILoadFileWriter>().ImplementedBy<LoadFileWriterRetryable>(),
				Component.For<IStateful>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileRetryableStreamWriter>()));
		}

		private void InstallStatistics(IWindsorContainer container)
		{
			container.Register(Component.For<IStateful, IFileProcessingStatistics, FilesStatistics>().ImplementedBy<FilesStatistics>());
			container.Register(Component.For<IStateful, IMetadataProcessingStatistics, MetadataStatistics>().ImplementedBy<MetadataStatistics>());
			container.Register(Component.For<IStateful, IDownloadProgress, IDownloadProgressManager, DownloadProgressManager>().ImplementedBy<DownloadProgressManager>());
		}
	}
}