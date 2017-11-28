using System.Collections;
using Castle.Windsor;
using kCura.WinEDDS.Container;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Service.Export;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ContainerFactory : IContainerFactory
	{
		public IWindsorContainer Create(ExportFile exportSettings, ArrayList columns, string columnHeader, string[] columnNamesInOrder, IExportManager exportManager, IUserNotification userNotification, IFileNameProvider fileNameProvider)
		{
			var container = new WindsorContainer();

			container.Install(new ExportInstaller(exportSettings, columns, columnHeader, columnNamesInOrder, exportManager, userNotification, fileNameProvider));

			return container;
		}
	}
}