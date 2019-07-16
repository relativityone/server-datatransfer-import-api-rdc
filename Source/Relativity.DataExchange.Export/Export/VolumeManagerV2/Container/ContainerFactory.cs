namespace Relativity.DataExchange.Export.VolumeManagerV2.Container
{
	using System;

	using Castle.MicroKernel.Resolvers.SpecializedResolvers;
	using Castle.Windsor;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Represents a class object factory to create Castle Windsor <see cref="IWindsorContainer"/> instances.
	/// Implements the <see cref="kCura.WinEDDS.Container.IContainerFactory" />
	/// </summary>
	/// <seealso cref="kCura.WinEDDS.Container.IContainerFactory" />
	public class ContainerFactory : IContainerFactory
	{
		private bool disposed;

		/// <summary>
		/// Initializes a new instance of the <see cref="ContainerFactory"/> class.
		/// </summary>
		public ContainerFactory()
		{
			this.Container = new WindsorContainer();
		}

		/// <summary>
		/// Gets the castle windsor container.
		/// </summary>
		/// <value>
		/// The <see cref="WindsorContainer"/> instance.
		/// </value>
		protected IWindsorContainer Container
		{
			get;
			private set;
		}

		public virtual IWindsorContainer Create(
			Exporter exporter,
			string[] columnNamesInOrder,
			bool useOldExport,
			ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(ExportStrings.ExportContainerDisposedExceptionMessage);
			}

			try
			{
				this.Container?.Dispose();
			}
			catch (Exception)
			{
				// The RDC uses a singleton pattern and is NOT designed to handle exceptions!
			}
			finally
			{
				// The RDC expects this object to get constructed every time this method is called.
				this.Container = new WindsorContainer();
			}

			if (!useOldExport)
			{
				this.Container.Kernel.Resolver.AddSubResolver(
					new CollectionResolver(this.Container.Kernel, true));
				this.Container.Install(
					new ExportInstaller(exporter, columnNamesInOrder, loadFileHeaderFormatterFactory));
			}

			this.OnCreate(exporter, columnNamesInOrder, useOldExport, loadFileHeaderFormatterFactory);
			return this.Container;
		}

		/// <summary>
		/// Called after the <see cref="Create"/> method is executed.
		/// </summary>
		/// <param name="exporter">
		/// The exporter.
		/// </param>
		/// <param name="columnNamesInOrder">
		/// The column names in order.
		/// </param>
		/// <param name="useOldExport">
		/// The flag indicating whether to use the old export design.
		/// </param>
		/// <param name="loadFileHeaderFormatterFactory">
		/// The factory that creates <see cref="ILoadFileHeaderFormatter"/> instances.
		/// </param>
		protected virtual void OnCreate(
			Exporter exporter,
			string[] columnNamesInOrder,
			bool useOldExport,
			ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this.Container != null)
				{
					this.Container.Dispose();
					this.Container = null;
				}

				this.disposed = true;
			}
		}
	}
}