using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using kCura.WinEDDS.Container;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ContainerFactory : IContainerFactory
	{
		public IWindsorContainer Create(Exporter exporter, string columnHeader, string[] columnNamesInOrder, bool useOldExport)
		{
			var container = new WindsorContainer();

			if (!useOldExport)
			{
				container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
				container.Install(new ExportInstaller(exporter, columnHeader, columnNamesInOrder));
			}

			return container;
		}
	}
}