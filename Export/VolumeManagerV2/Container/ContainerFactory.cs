using Castle.Windsor;
using kCura.WinEDDS.Container;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Service.Export;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ContainerFactory : IContainerFactory
	{
		public IWindsorContainer Create(ExportFile exportSettings, IExportManager exportManager, IUserNotification userNotification)
		{
			var container = new WindsorContainer();

			container.Install(new ExportInstaller(exportSettings, exportManager, userNotification));

			return container;
		}
	}
}