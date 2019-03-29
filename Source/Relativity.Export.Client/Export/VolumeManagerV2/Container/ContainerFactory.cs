namespace Relativity.Export.VolumeManagerV2.Container
{
	using Castle.MicroKernel.Resolvers.SpecializedResolvers;
	using Castle.Windsor;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;

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