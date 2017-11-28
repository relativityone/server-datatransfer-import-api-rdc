using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Requests;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation;
using kCura.WinEDDS.Core.IO;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Exporters.Validator;
using kCura.WinEDDS.Service.Export;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ExportInstaller : IWindsorInstaller
	{
		private readonly ExportFile _exportSettings;
		private readonly ArrayList _columns;
		private readonly string _columnHeader;
		private readonly string[] _columnNamesInOrder;
		private readonly IExportManager _exportManager;
		private readonly IUserNotification _userNotification;
		private readonly IFileNameProvider _fileNameProvider;

		public ExportInstaller(ExportFile exportSettings, ArrayList columns, string columnHeader, string[] columnNamesInOrder, IExportManager exportManager,
			IUserNotification userNotification, IFileNameProvider fileNameProvider)
		{
			_exportSettings = exportSettings;
			_columns = columns;
			_columnHeader = columnHeader;
			_columnNamesInOrder = columnNamesInOrder;
			_exportManager = exportManager;
			_userNotification = userNotification;
			_fileNameProvider = fileNameProvider;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			InstallLogger(container);
			InstallValidators(container);
			InstallDataSize(container);
			InstallFieldService(container);
			InstallUtils(container);
			InstallLabelManager(container);

			container.Register(Component.For<ExportFile>().Instance(_exportSettings).LifestyleSingleton());
			container.Register(Component.For<ExportColumns>().UsingFactoryMethod(k => new ExportColumns(_columns, k.Resolve<IFieldLookupService>())));
			container.Register(Component.For<IExportManager>().Instance(_exportManager).LifestyleSingleton());

			container.Register(Component.For<ErrorFileDestinationPath>().ImplementedBy<ErrorFileDestinationPath>());
			container.Register(Component.For<IErrorFile>().UsingFactoryMethod(k => k.Resolve<ErrorFileDestinationPath>()));

			container.Register(Component.For<IFileNameProvider>().Instance(_fileNameProvider));

			container.Register(Component.For<NativeExportRequestBuilder>().ImplementedBy<NativeExportRequestBuilder>());
			container.Register(Component.For<ImageExportRequestBuilder>().ImplementedBy<ImageExportRequestBuilder>());

			container.Register(Component.For<IBatchExporter>().ImplementedBy<BatchExporter>());
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

			container.Register(Component.For<IUserNotification>().Instance(_userNotification));
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
				.UsingFactoryMethod(k => k.Resolve<FieldServiceFactory>().Create(_exportSettings, _columns, _columnHeader, _columnNamesInOrder)).LifestyleSingleton());

			container.Register(Component.For<IFieldLookupService>().UsingFactoryMethod(k => k.Resolve<FieldService>()));
			container.Register(Component.For<IFieldService>().UsingFactoryMethod(k => k.Resolve<FieldService>()));
		}

		private static void InstallUtils(IWindsorContainer container)
		{
			container.Register(Component.For<IFileHelper>().ImplementedBy<LongPathFileHelper>());
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
		}
	}
}