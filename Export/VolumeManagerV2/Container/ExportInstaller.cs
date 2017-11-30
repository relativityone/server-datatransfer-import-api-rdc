using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation;
using kCura.WinEDDS.Core.IO;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using kCura.WinEDDS.IO;
using kCura.WinEDDS.Service.Export;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ExportInstaller : IWindsorInstaller
	{
		private readonly Exporter _exporter;
		private readonly string _columnHeader;
		private readonly string[] _columnNamesInOrder;

		public ExportInstaller(Exporter exporter, string columnHeader, string[] columnNamesInOrder)
		{
			_exporter = exporter;
			_columnHeader = columnHeader;
			_columnNamesInOrder = columnNamesInOrder;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			InstallLogger(container);
			InstallValidators(container);
			InstallDataSize(container);
			InstallFieldService(container);
			InstallUtils(container);
			InstallLabelManager(container);

			container.Register(Component.For<ExportFile>().Instance(_exporter.Settings).LifestyleSingleton());
			container.Register(Component.For<ExportColumns>().UsingFactoryMethod(k => new ExportColumns(_exporter.Columns, k.Resolve<IFieldLookupService>())));
			container.Register(Component.For<IExportManager>().Instance(_exporter.ExportManager).LifestyleSingleton());
			container.Register(Component.For<IStatus>().Instance(_exporter));

			container.Register(Component.For<ImagesRollupFactory>().ImplementedBy<ImagesRollupFactory>());
			container.Register(Component.For<IImagesRollup>().UsingFactoryMethod(k => k.Resolve<ImagesRollupFactory>().Create(_exporter.Settings)));

			container.Register(Component.For<ErrorFileDestinationPath>().ImplementedBy<ErrorFileDestinationPath>());
			container.Register(Component.For<IErrorFile>().UsingFactoryMethod(k => k.Resolve<ErrorFileDestinationPath>()));

			container.Register(Component.For<IFileNameProvider>().Instance(_exporter.FileNameProvider));

			container.Register(Component.For<NativeExportRequestBuilder>().ImplementedBy<NativeExportRequestBuilder>());
			container.Register(Component.For<ImageExportRequestBuilder>().ImplementedBy<ImageExportRequestBuilder>());
			container.Register(Component.For<TextExportRequestBuilder>().ImplementedBy<TextExportRequestBuilder>());
			container.Register(Component.For<DownloadedTextFilesRepository>().ImplementedBy<DownloadedTextFilesRepository>());

			container.Register(Component.For<ExportTapiBridgeFactory>().ImplementedBy<ExportTapiBridgeFactory>());

			container.Register(Component.For<IBatchExporter>().ImplementedBy<BatchExporter>());
			container.Register(Component.For<FilesDownloader>().ImplementedBy<FilesDownloader>());

			container.Register(Component.For<FilePathProviderFactory>().ImplementedBy<FilePathProviderFactory>());
			container.Register(Component.For<IFilePathProvider>().UsingFactoryMethod(k => k.Resolve<FilePathProviderFactory>().Create(_exporter.Settings)));

			container.Register(Component.For<ImageLoadFileFactory>().ImplementedBy<ImageLoadFileFactory>());
			container.Register(Component.For<Metadata.Images.ImageLoadFile>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileFactory>().Create(_exporter.Settings)));
			container.Register(Component.For<LoadFileEntryFactory>().ImplementedBy<LoadFileEntryFactory>());
			container.Register(Component.For<ILoadFileEntry>().UsingFactoryMethod(k => k.Resolve<LoadFileEntryFactory>().Create(_exporter.Settings)));

			container.Register(Component.For<StreamFactory>().ImplementedBy<StreamFactory>());

			container.Register(Component.For<ImageLoadFileDestinationPath>().ImplementedBy<ImageLoadFileDestinationPath>());
			container.Register(Component.For<StatisticsWrapper>().UsingFactoryMethod(k => new StatisticsWrapper(_exporter._statistics)));

			container.Register(Component.For<Settings.Config>().ImplementedBy<Settings.Config>());

			container.Register(Component.For<WritersRetryPolicy>().ImplementedBy<WritersRetryPolicy>());

			container.Register(Component.For<ImageLoadFileWriterFactory>().ImplementedBy<ImageLoadFileWriterFactory>());
			container.Register(Component.For<ImageLoadFileWriter>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileWriterFactory>().Create()));
		}

		private void InstallLogger(IWindsorContainer container)
		{
			container.Register(Component.For<ILog>().Instance(new NullLogger()).LifestyleSingleton());
		}

		private void InstallValidators(IWindsorContainer container)
		{
			container.Register(Component.For<ExportPermissionCheck>().ImplementedBy<ExportPermissionCheck>());
			container.Register(Component.For<FilesOverwriteValidator>().ImplementedBy<FilesOverwriteValidator>());
			container.Register(Component.For<VolumeAndSubdirectoryValidator>().ImplementedBy<VolumeAndSubdirectoryValidator>());
			container.Register(Component.For<PaddingWarningValidator>().ImplementedBy<PaddingWarningValidator>());
			container.Register(Component.For<IExportValidation>().ImplementedBy<ExportValidation>());

			container.Register(Component.For<IUserNotification>().Instance(_exporter.InteractionManager));
		}

		private void InstallDataSize(IWindsorContainer container)
		{
			container.Register(Component.For<ImageExportableSize>().ImplementedBy<ImageExportableSize>());
			container.Register(Component.For<NativeExportableSize>().ImplementedBy<NativeExportableSize>());
			container.Register(Component.For<TextExportableSize>().ImplementedBy<TextExportableSize>());
			container.Register(Component.For<IObjectExportableSize>().ImplementedBy<ObjectExportableSize>());
		}

		private void InstallFieldService(IWindsorContainer container)
		{
			container.Register(Component.For<FieldServiceFactory>().ImplementedBy<FieldServiceFactory>());
			container.Register(Component.For<FieldService>()
				.UsingFactoryMethod(k => k.Resolve<FieldServiceFactory>().Create(_exporter.Settings, _exporter.Columns, _columnHeader, _columnNamesInOrder)).LifestyleSingleton());

			container.Register(Component.For<IFieldLookupService>().UsingFactoryMethod(k => k.Resolve<FieldService>()));
			container.Register(Component.For<IFieldService>().UsingFactoryMethod(k => k.Resolve<FieldService>()));
		}

		private static void InstallUtils(IWindsorContainer container)
		{
			container.Register(Component.For<IFileHelper>().ImplementedBy<LongPathFileHelper>());
			container.Register(Component.For<IFileStreamFactory>().ImplementedBy<FileStreamFactory>());
			container.Register(Component.For<IDirectoryHelper>().ImplementedBy<LongPathDirectoryHelper>());
		}

		private static void InstallLabelManager(IWindsorContainer container)
		{
			container.Register(Component.For<LabelManager>().ImplementedBy<LabelManager>());
			container.Register(Component.For<TrueVolumeManager>().ImplementedBy<TrueVolumeManager>());
			container.Register(Component.For<SubdirectoryManager>().ImplementedBy<SubdirectoryManager>());
			container.Register(Component.For<IVolume>().UsingFactoryMethod(k => k.Resolve<TrueVolumeManager>()));
			container.Register(Component.For<ISubdirectory>().UsingFactoryMethod(k => k.Resolve<SubdirectoryManager>()));
			container.Register(Component.For<ISubdirectoryManager>().UsingFactoryMethod(k => k.Resolve<SubdirectoryManager>()));
			container.Register(Component.For<IDirectoryManager>().ImplementedBy<DirectoryManager>());
		}
	}
}