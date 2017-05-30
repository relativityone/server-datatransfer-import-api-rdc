using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Managers;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks;
using kCura.WinEDDS.Core.Import.Tasks.Helpers;
using kCura.WinEDDS.Core.IO;
using kCura.WinEDDS.Importers;

namespace kCura.WinEDDS.Core.Installer
{
	public class CoreInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			RegisterErrorHandling(container);
			RegisterScopedForBatch(container);
			RegisterManagers(container);
			RegisterStatistics(container);

			container.Register(Component.For<ITransferConfig>().ImplementedBy<TransferConfig>().LifestyleSingleton());
			container.Register(Component.For<IImportBatchJobFactory>().ImplementedBy<ImportBatchJobFactory>().LifestyleSingleton());
			container.Register(Component.For<IImportJobFactory>().ImplementedBy<ImportJobFactory>().LifestyleSingleton());
			container.Register(Component.For<IImportStatusManager>().ImplementedBy<ImportStatusManager>().LifestyleSingleton());

			container.Register(Component.For<ILoadFileImporterFactory>().ImplementedBy<LoadFileImporterFactory>().LifestyleTransient());

			container.Register(Component.For<IFileInfoProvider>().ImplementedBy<FileInfoProvider>().LifestyleTransient());
			container.Register(Component.For<IRepositoryFilePathHelper>().ImplementedBy<RepositoryFilePathHelper>().LifestyleTransient());

			//TODO handle better
			container.Register(Component.For<IFolderCache>().UsingFactoryMethod(k =>
			{
				IImporterManagers importerManagers = k.Resolve<IImporterManagers>();
				ImportContext importContext = k.Resolve<ImportContext>();
				return new FolderCache(importerManagers.FolderManager, importContext.Settings.FolderId, importContext.Settings.LoadFile.CaseInfo.ArtifactID);
			}).LifestyleTransient());

			//TODO move to utility project
			container.Register(Component.For<IPathHelper>().ImplementedBy<PathHelper>().LifestyleSingleton());
			container.Register(Component.For<IFileHelper>().ImplementedBy<LongPathFileHelper>().LifestyleSingleton());
			container.Register(Component.For<IDateTimeHelper>().ImplementedBy<DateTimeHelper>().LifestyleSingleton());
			container.Register(Component.For<ICancellationProvider>().ImplementedBy<CancellationProvider>().LifestyleSingleton());
		}

		private static void RegisterStatistics(IWindsorContainer container)
		{
			container.Register(Component.For<IMetadataStatisticsHandler>().ImplementedBy<MetadataStatisticsHandler>().LifestyleSingleton());
			container.Register(Component.For<IBulkImportStatisticsHandler>().ImplementedBy<BulkImportStatisticsHandler>().LifestyleSingleton());
			container.Register(Component.For<IServerErrorStatisticsHandler>().ImplementedBy<ServerErrorStatisticsHandler>().LifestyleSingleton());
			container.Register(Component.For<IJobFinishStatisticsHandler>().ImplementedBy<JobFinishStatisticsHandler>().LifestyleSingleton());
		}

		private static void RegisterErrorHandling(IWindsorContainer container)
		{
			container.Register(Component.For<AllErrorsContainer>().ImplementedBy<AllErrorsContainer>().LifestyleSingleton());
			container.Register(Component.For<ClientErrorLineContainer>().ImplementedBy<ClientErrorLineContainer>().LifestyleSingleton());
			container.Register(Component.For<ErrorReporter>().ImplementedBy<ErrorReporter>().LifestyleSingleton());

			container.Register(Component.For<IAllErrors>().UsingFactoryMethod(k => k.Resolve<AllErrorsContainer>()).LifestyleTransient());
			container.Register(Component.For<IClientErrors>().UsingFactoryMethod(k => k.Resolve<ClientErrorLineContainer>()).LifestyleTransient());

			container.Register(Component.For<IErrorContainer>().UsingFactoryMethod(k => ErrorContainerFactory.Create(container)).LifestyleSingleton());

			container.Register(Component.For<ErrorFileNames>().ImplementedBy<ErrorFileNames>().LifestyleSingleton());

			container.Register(Component.For<IUploadErrors>().ImplementedBy<UploadErrors>().LifestyleTransient());

			container.Register(Component.For<IErrorManagerFactory>().ImplementedBy<ErrorManagerFactory>().LifestyleTransient());

			container.Register(Component.For<IImportExceptionHandlerExec>().ImplementedBy<ImportExceptionHandlerExec>().LifestyleSingleton());

			container.Register(Component.For<IServerErrorFileDownloader>().ImplementedBy<ServerErrorFileDownloader>().LifestyleTransient());
			container.Register(Component.For<IServerErrorFile>().ImplementedBy<ServerErrorFile>().LifestyleTransient());
			container.Register(Component.For<IServerErrorManager>().ImplementedBy<ServerErrorManager>().LifestyleTransient());
			container.Register(Component.For<IErrorFileDownloaderFactory>().ImplementedBy<ErrorFileDownloaderFactory>().LifestyleTransient());
		}

		private static void RegisterScopedForBatch(IWindsorContainer container)
		{
			RegisterTasks(container);

			container.Register(Component.For<IImportBatchJob>().ImplementedBy<ImportBatchJob>().LifestyleScoped());
			container.Register(Component.For<ImportBatchContextProvider>().ImplementedBy<ImportBatchContextProvider>().LifestyleScoped());
			container.Register(Component.For<ImportBatchContext>().UsingFactoryMethod(k => k.Resolve<ImportBatchContextProvider>().ImportBatchContext).LifestyleScoped());
			container.Register(Component.For<ImportContext>().UsingFactoryMethod(k => k.Resolve<ImportBatchContextProvider>().ImportBatchContext.ImportContext).LifestyleScoped());
			container.Register(Component.For<INativeLoadInfoFactory>().ImplementedBy<NativeLoadInfoFactory>().LifestyleScoped());
		}

		private static void RegisterTasks(IWindsorContainer container)
		{
			//TODO document vs rdo
			container.Register(Component.For<IImportFoldersTask>().ImplementedBy<ImportDocumentFoldersTask>().LifestyleScoped());

			container.Register(Component.For<IImportNativesAnalyzer>().ImplementedBy<ImportNativesAnalyzer>().LifestyleScoped());
			container.Register(Component.For<IImportNativesTask>().ImplementedBy<ImportNativesTask>().LifestyleScoped());
			container.Register(Component.For<IPushMetadataFilesTask>().ImplementedBy<PushMetadataFilesTask>().LifestyleScoped());
			container.Register(Component.For<IImportPrepareMetadataTask>().ImplementedBy<ImportPrepareMetadataTask>().LifestyleScoped());

			container.Register(Component.For<IMetadataFilesServerExecution>().ImplementedBy<MetadataFilesServerExecution>().LifestyleScoped());
		}

		private static void RegisterManagers(IWindsorContainer container)
		{
			container.Register(Component.For<IBulkImportManager>().UsingFactoryMethod(k => k.Resolve<IImporterManagers>().BulkImportManager).LifestyleTransient());
		}
	}
}