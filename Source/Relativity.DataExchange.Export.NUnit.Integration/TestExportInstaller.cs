// -----------------------------------------------------------------------------------------------------
// <copyright file="TestExportInstaller.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to install and register non-mocked and mocked components with the Castle Windsor container.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit.Integration
{
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Container;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents a class object to install and register non-mocked and mocked components with the Castle Windsor container.
	/// Implements the <see cref="Relativity.DataExchange.Export.VolumeManagerV2.Container.ExportInstaller" />.
	/// </summary>
	/// <seealso cref="Relativity.DataExchange.Export.VolumeManagerV2.Container.ExportInstaller" />
	public class TestExportInstaller : ExportInstaller
	{
		private readonly IMockServiceRegistration mockServiceRegistration;

		public TestExportInstaller(
			Exporter exporter,
			string[] columnNamesInOrder,
			ILoadFileHeaderFormatterFactory factory)
			: this(exporter, columnNamesInOrder, factory, null)
		{
		}

		public TestExportInstaller(
			Exporter exporter,
			string[] columnNamesInOrder,
			ILoadFileHeaderFormatterFactory factory,
			IMockServiceRegistration registration)
			: base(exporter, columnNamesInOrder, factory)
		{
			this.mockServiceRegistration = registration;
		}

		protected override void OnInstall(IWindsorContainer container, IConfigurationStore store)
		{
			base.OnInstall(container, store);
			this.mockServiceRegistration?.Register(container);
		}
	}
}