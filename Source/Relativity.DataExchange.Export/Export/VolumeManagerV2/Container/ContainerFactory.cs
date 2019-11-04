namespace Relativity.DataExchange.Export.VolumeManagerV2.Container
{
	using System;

	using Castle.MicroKernel.Resolvers.SpecializedResolvers;
	using Castle.Windsor;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;

	using Relativity.Logging;

	/// <summary>
	/// Represents a class object factory to create Castle Windsor <see cref="IWindsorContainer"/> instances.
	/// Implements the <see cref="kCura.WinEDDS.Container.IContainerFactory" />
	/// </summary>
	/// <seealso cref="kCura.WinEDDS.Container.IContainerFactory" />
	public class ContainerFactory : IContainerFactory
	{
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerFactory"/> class.
		/// </summary>
		[Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")]
		public ContainerFactory()
			: this(RelativityLogger.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerFactory"/> class.
		/// </summary>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		public ContainerFactory(ILog logger)
		{
			this.logger = logger;
		}

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
				container.Install(
					new ExportInstaller(exporter, columnNamesInOrder, loadFileHeaderFormatterFactory, this.logger));
			}

			return container;
		}
	}
}