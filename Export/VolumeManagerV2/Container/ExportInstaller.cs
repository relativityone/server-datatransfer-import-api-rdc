using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
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
		private readonly IExportManager _exportManager;
		private readonly IUserNotification _userNotification;

		public ExportInstaller(ExportFile exportSettings, IExportManager exportManager, IUserNotification userNotification)
		{
			_exportSettings = exportSettings;
			_exportManager = exportManager;
			_userNotification = userNotification;
		}

		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			InstallLogger(container);
			InstallValidators(container);

			container.Register(Component.For<ExportFile>().Instance(_exportSettings).LifestyleSingleton());
			container.Register(Component.For<IExportManager>().Instance(_exportManager).LifestyleSingleton());

			container.Register(Component.For<IFileHelper>().ImplementedBy<LongPathFileHelper>());
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
	}
}