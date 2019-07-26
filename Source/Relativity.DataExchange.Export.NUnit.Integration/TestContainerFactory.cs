// -----------------------------------------------------------------------------------------------------
// <copyright file="TestContainerFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents the container used by the integration tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using Castle.MicroKernel.Resolvers.SpecializedResolvers;
	using Castle.Windsor;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Container;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Container;

	/// <summary>
	/// Represents a class object factory to create a test container that installs <see cref="ExportInstaller"/> but also allows tests to register mocks.
	/// Implements the <see cref="kCura.WinEDDS.Container.IContainerFactory" />.
	/// </summary>
	/// <seealso cref="kCura.WinEDDS.Container.IContainerFactory" />
	public class TestContainerFactory : IContainerFactory
	{
		private readonly IWindsorContainer container;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestContainerFactory"/> class.
		/// </summary>
		/// <param name="container">
		/// The Castle Windsor container.
		/// </param>
		public TestContainerFactory(IWindsorContainer container)
		{
			this.container = container.ThrowIfNull(nameof(container));
		}

		public virtual IWindsorContainer Create(
			Exporter exporter,
			string[] columnNamesInOrder,
			bool useOldExport,
			ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
			// Note: bypass the "real" ContainerFactory class to install directly into the test container.
			if (!useOldExport)
			{
				this.container.Kernel.Resolver.AddSubResolver(new CollectionResolver(this.container.Kernel, true));
				this.container.Install(new ExportInstaller(exporter, columnNamesInOrder, loadFileHeaderFormatterFactory));
			}

			return this.container;
		}
	}
}