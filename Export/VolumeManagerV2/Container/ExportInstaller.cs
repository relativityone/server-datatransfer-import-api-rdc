using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using kCura.WinEDDS.IO;
using kCura.WinEDDS.Service.Export;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	/// <summary>
	///     TODO clean this up!
	/// </summary>
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
			container.Register(Component.For<StatisticsWrapper>().UsingFactoryMethod(k => new StatisticsWrapper(_exporter._statistics)));
			container.Register(Component.For<ILog>().Instance(new NullLogger()).LifestyleSingleton());

			container.Register(Component.For<FileDownloader>().UsingFactoryMethod(k => new FileDownloader(ExportSettings.Credential,
				$"{ExportSettings.CaseInfo.DocumentPath}\\EDDS{ExportSettings.CaseInfo.ArtifactID}", ExportSettings.CaseInfo.DownloadHandlerURL, ExportSettings.CookieContainer,
				Service.Settings.AuthenticationToken)));
		}

		private void InstallCustom(IWindsorContainer container)
		{
			InstallFieldService(container);
			InstallDirectory(container);
			InstallImages(container);
			InstallNatives(container);
			InstallLongText(container);

			// OTHER
			container.Register(Component.For<IErrorFile>().UsingFactoryMethod(k => k.Resolve<ErrorFileDestinationPath>()));
			container.Register(Component.For<IFilePathTransformer>().UsingFactoryMethod(k => k.Resolve<FilePathTransformerFactory>().Create(ExportSettings)));
			container.Register(Component.For<FilesDownloader>().UsingFactoryMethod(k => k.Resolve<FilesDownloaderFactory>().Create(ExportSettings)));
		}

		private void InstallFieldService(IWindsorContainer container)
		{
			container.Register(Component.For<IFieldLookupService>().UsingFactoryMethod(k => k.Resolve<FieldService>()));
			container.Register(Component.For<IFieldService>().UsingFactoryMethod(k => k.Resolve<FieldService>()));
			container.Register(Component.For<FieldService>()
				.UsingFactoryMethod(k => k.Resolve<FieldServiceFactory>().Create(ExportSettings, _exporter.Columns, _columnHeader, _columnNamesInOrder)).LifestyleSingleton());
		}

		private void InstallDirectory(IWindsorContainer container)
		{
			container.Register(Component.For<IVolume>().UsingFactoryMethod(k => k.Resolve<TrueVolumeManager>()));
			container.Register(Component.For<ISubdirectory>().UsingFactoryMethod(k => k.Resolve<SubdirectoryManager>()));
			container.Register(Component.For<ISubdirectoryManager>().UsingFactoryMethod(k => k.Resolve<SubdirectoryManager>()));
		}

		private void InstallImages(IWindsorContainer container)
		{
			container.Register(Component.For<IImagesRollup>().UsingFactoryMethod(k => k.Resolve<ImagesRollupFactory>().Create(ExportSettings)));
			container.Register(Component.For<Metadata.Images.ImageLoadFile>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileFactory>().Create(ExportSettings)));
			container.Register(Component.For<ILoadFileEntry>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileEntryFactory>().Create(ExportSettings)));
			container.Register(Component.For<ImageLoadFileWriter>().UsingFactoryMethod(k => k.Resolve<ImageLoadFileWriterFactory>().Create()));
		}

		private void InstallNatives(IWindsorContainer container)
		{
			container.Register(Component.For<ILoadFileCellFormatter>().UsingFactoryMethod(k => k.Resolve<LoadFileCellFormatterFactory>().Create(ExportSettings)));
			container.Register(Component.For<LoadFileWriter>().UsingFactoryMethod(k => k.Resolve<LoadFileWriterFactory>().Create()));
		}

		private void InstallLongText(IWindsorContainer container)
		{
			container.Register(Component.For<LongTextRepositoryBuilder>().UsingFactoryMethod(k => k.Resolve<LongTextRepositoryBuilderFactory>().Create(ExportSettings)));
			container.Register(Component.For<IFullTextLoadFileEntry>().UsingFactoryMethod(k => k.Resolve<FullTextLoadFileEntryFactory>().Create(ExportSettings)));
			container.Register(Component.For<ILongTextHandler>().UsingFactoryMethod(k => k.Resolve<LongTextHandlerFactory>().Create(ExportSettings)));
			container.Register(Component.For<IDelimiter>().UsingFactoryMethod(k => k.Resolve<DelimiterFactory>().Create(ExportSettings)));
		}
	}
}