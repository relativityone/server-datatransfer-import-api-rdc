namespace Relativity.DataExchange.Export.VolumeManagerV2.Container
{
	using Castle.MicroKernel.Resolvers.SpecializedResolvers;
	using Castle.Windsor;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;

	/// <summary>
	/// Represents a class object factory to create Castle Windsor <see cref="IWindsorContainer"/> instances.
	/// Implements the <see cref="kCura.WinEDDS.Container.IContainerFactory" />
	/// </summary>
	/// <seealso cref="kCura.WinEDDS.Container.IContainerFactory" />
	public class ContainerFactory : IContainerFactory
	{
		public virtual IWindsorContainer Create(
			Exporter exporter,
			string[] columnNamesInOrder,
			bool useOldExport,
			ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
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