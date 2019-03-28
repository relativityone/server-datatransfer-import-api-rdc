using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using kCura.WinEDDS.Container;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ContainerFactory : IContainerFactory
	{
		public IWindsorContainer Create(Exporter exporter, string[] columnNamesInOrder, bool useOldExport, ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
			var container = new WindsorContainer();

			if (!useOldExport)
			{
				container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
				container.Install(new ExportInstaller(exporter, columnNamesInOrder, loadFileHeaderFormatterFactory));
			}

			return container;
		}
	}
}