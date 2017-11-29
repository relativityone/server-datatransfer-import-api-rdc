using Castle.Windsor;
using kCura.WinEDDS.Container;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ContainerFactory : IContainerFactory
	{
		public IWindsorContainer Create(Exporter exporter, string columnHeader, string[] columnNamesInOrder)
		{
			var container = new WindsorContainer();

			container.Install(new ExportInstaller(exporter, columnHeader, columnNamesInOrder));

			return container;
		}
	}
}