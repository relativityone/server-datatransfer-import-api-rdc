using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using kCura.WinEDDS.Container;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Container
{
	public class ContainerFactory : IContainerFactory
	{
		public IWindsorContainer Create(Exporter exporter, string[] columnNamesInOrder, bool useOldExport, string columnHeaderString)
		{
			var container = new WindsorContainer();

			if (!useOldExport)
			{
				container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
				container.Install(new ExportInstaller(exporter, columnNamesInOrder, columnHeaderString));
			}

			return container;
		}
	}
}