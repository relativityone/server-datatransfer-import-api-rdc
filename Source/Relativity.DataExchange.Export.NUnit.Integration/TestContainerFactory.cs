﻿// -----------------------------------------------------------------------------------------------------
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
	using Relativity.Logging;

	/// <summary>
	/// Represents a class object factory to create a test container that installs <see cref="ExportInstaller"/> but also allows tests to register mocks.
	/// Implements the <see cref="kCura.WinEDDS.Container.IContainerFactory" />.
	/// </summary>
	/// <seealso cref="kCura.WinEDDS.Container.IContainerFactory" />
	public class TestContainerFactory : IContainerFactory
	{
		private readonly IWindsorContainer container;
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestContainerFactory"/> class.
		/// </summary>
		/// <param name="container">
		/// The Castle Windsor container.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		public TestContainerFactory(IWindsorContainer container, ILog logger)
		{
			this.container = container.ThrowIfNull(nameof(container));
			this.logger = logger.ThrowIfNull(nameof(logger));
		}

		public virtual IWindsorContainer Create(
			Exporter exporter,
			string[] columnNamesInOrder,
			ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
			// Note: bypass the "real" ContainerFactory class to install directly into the test container.
			this.container.Kernel.Resolver.AddSubResolver(new CollectionResolver(this.container.Kernel, true));
			this.container.Install(
				new ExportInstaller(exporter, columnNamesInOrder, loadFileHeaderFormatterFactory, this.logger));

			return this.container;
		}
	}
}