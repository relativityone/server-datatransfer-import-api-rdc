// -----------------------------------------------------------------------------------------------------
// <copyright file="ApplicationVersionServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="kCura.WinEDDS.Service.ApplicationVersionService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Integration
{
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="kCura.WinEDDS.Service.ApplicationVersionService"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi]
	public class ApplicationVersionServiceTests : WebServiceTestsBase
	{
		[Test]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.WebService)]
		public async Task ShouldGetTheBackendApiVersionAsync()
		{
			IApplicationVersionService service =
				new kCura.WinEDDS.Service.ApplicationVersionService(
					this.RelativityInstance,
					this.AppSettings,
					this.Logger.Object);
			System.Version version = await service.GetImportExportWebApiVersionAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
			Assert.That(version, Is.Not.Null);
			Assert.That(version.Major, Is.Positive);
		}

		[Test]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.WebService)]
		public async Task ShouldGetTheRelativityVersionAsync()
		{
			IApplicationVersionService service = this.CreateService();
			System.Version version = await service.GetRelativityVersionAsync(this.CancellationTokenSource.Token).ConfigureAwait(false);
			Assert.That(version, Is.Not.Null);
			Assert.That(version.Major, Is.Positive);
		}

		private IApplicationVersionService CreateService()
		{
			return new kCura.WinEDDS.Service.ApplicationVersionService(
				this.RelativityInstance,
				this.AppSettings,
				this.Logger.Object);
		}
	}
}