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
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Container;

	/// <summary>
	/// Represents a class object to extend the existing factory and install <see cref="TestExportInstaller"/>.
	/// </summary>
	public class TestContainerFactory : ContainerFactory
	{
		private readonly TestMockServiceRegistration mockServiceRegistration;

		public TestContainerFactory(TestMockServiceRegistration registration)
		{
			this.mockServiceRegistration = registration;
		}

		public override IWindsorContainer Create(
			Exporter exporter,
			string[] columnNamesInOrder,
			bool useOldExport,
			ILoadFileHeaderFormatterFactory loadFileHeaderFormatterFactory)
		{
			if (!useOldExport)
			{
				this.Container.Kernel.Resolver.AddSubResolver(new CollectionResolver(this.Container.Kernel, true));
				this.Container.Install(
					new TestExportInstaller(
						exporter,
						columnNamesInOrder,
						loadFileHeaderFormatterFactory,
						this.mockServiceRegistration));
			}

			return this.Container;
		}
	}
}