using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
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

		protected ExportFile ExportSettings => _exporter.Settings;

		public ExportInstaller(Exporter exporter, string columnHeader, string[] columnNamesInOrder)
		{
			_exporter = exporter;
			_columnHeader = columnHeader;
			_columnNamesInOrder = columnNamesInOrder;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			InstallFromWinEdds(container);
			InstallConnectionToWinEdds(container);
			InstallTemporary(container);
			InstallCustom(container);

			//TODO extract interfaces and then remove Self()
			container.Register(Classes.FromThisAssembly().Pick().WithService.DefaultInterfaces().WithService.Self());
		}

		private void InstallFromWinEdds(IWindsorContainer container)
		{
			container.Register(Component.For<PaddingWarningValidator>().ImplementedBy<PaddingWarningValidator>());
			container.Register(Component.For<IFileStreamFactory>().ImplementedBy<FileStreamFactory>());
			container.Register(Component.For<IExportFileDownloaderStatus, ExportFileDownloaderStatus>().ImplementedBy<ExportFileDownloaderStatus>());
			container.Register(Component.For<ILoadFileCellFormatter>().UsingFactoryMethod(k => k.Resolve<LoadFileCellFormatterFactory>().Create(ExportSettings)));
		}

		private void InstallConnectionToWinEdds(IWindsorContainer container)
		{
			container.Register(Component.For<ExportFile>().Instance(ExportSettings));
			container.Register(Component.For<IExportManager>().Instance(_exporter.ExportManager));
			container.Register(Component.For<IStatus>().Instance(_exporter));
			container.Register(Component.For<IFileNameProvider>().Instance(_exporter.FileNameProvider));
			container.Register(Component.For<IUserNotification>().Instance(_exporter.InteractionManager));
		}

		/// <summary>
		///     TODO remove
		/// </summary>
		/// <param name="container"></param>
		private void InstallTemporary(IWindsorContainer container)
		{
			container.Register(Component.For<ILog>().Instance(new NullLogger()).LifestyleSingleton());
		}

		private void InstallCustom(IWindsorContainer container)
		{
			InstallFieldService(container);
			InstallDirectory(container);
			InstallNatives(container);
			InstallImages(container);
			InstallLongText(container);
			InstallStatefulComponents(container);

			// OTHER
			container.Register(Component.For<IErrorFile>().UsingFactoryMethod(k => k.Resolve<ErrorFileDestinationPath>()));
			container.Register(Component.For<IFilePathTransformer>().UsingFactoryMethod(k => k.Resolve<FilePathTransformerFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IBatchValidator>().UsingFactoryMethod(k => k.Resolve<BatchValidatorFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileWriter>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileWriterFactory>().Create(ExportSettings, container)));
		}

		private void InstallFieldService(IWindsorContainer container)
		{
			container.Register(Component.For<IFieldLookupService, IFieldService, FieldService>()
				.UsingFactoryMethod(k => k.Resolve<FieldServiceFactory>().Create(ExportSettings, _exporter.Columns, _columnHeader, _columnNamesInOrder)));
		}

		private void InstallDirectory(IWindsorContainer container)
		{
			container.Register(Component.For<IVolume, Directories.VolumeManager>().ImplementedBy<Directories.VolumeManager>());
			container.Register(Component.For<ISubdirectoryManager, ISubdirectory, SubdirectoryManager>().ImplementedBy<SubdirectoryManager>());
		}

		private void InstallNatives(IWindsorContainer container)
		{
			container.Register(Component.For<IRepository, NativeRepository>().ImplementedBy<NativeRepository>());
			container.Register(Component.For<NativeRepositoryBuilder>().UsingFactoryMethod(k => k.Resolve<NativeRepositoryBuilderFactory>().Create(ExportSettings, container)));
		}

		private void InstallImages(IWindsorContainer container)
		{
			container.Register(Component.For<IImagesRollup>().UsingFactoryMethod(k => k.Resolve<ImagesRollupFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileMetadataBuilder>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileMetadataBuilderFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IImageLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileEntryFactory>().Create(ExportSettings, container)));

			container.Register(Component.For<IRepository, ImageRepository>().ImplementedBy<ImageRepository>());
			container.Register(Component.For<ImageRepositoryBuilder>().UsingFactoryMethod(k => k.Resolve<ImageRepositoryBuilderFactory>().Create(ExportSettings, container)));
		}

		private void InstallLongText(IWindsorContainer container)
		{
			container.Register(Component.For<IRepository, LongTextRepository>().ImplementedBy<LongTextRepository>());
			container.Register(Component.For<LongTextRepositoryBuilder>().UsingFactoryMethod(k => k.Resolve<LongTextRepositoryBuilderFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IFullTextLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<FullTextLoadFileEntryFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<ILongTextHandler>().UsingFactoryMethod(k => k.Resolve<LongTextHandlerFactory>().Create(ExportSettings, container)));
			container.Register(Component.For<IDelimiter>().UsingFactoryMethod(k => k.Resolve<DelimiterFactory>().Create(ExportSettings)));
		}

		private void InstallStatefulComponents(IWindsorContainer container)
		{
			container.Register(Component.For<IStateful, ILoadFileWriter>().ImplementedBy<LoadFileWriterRetryable>(),
				Component.For<IStateful, IImageLoadFileWriter>().ImplementedBy<ImageLoadFileWriterRetryable>());
		}
	}
}