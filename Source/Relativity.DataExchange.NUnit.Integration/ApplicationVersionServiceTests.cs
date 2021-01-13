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
	[TestType.MainFlow]
	[TestType.DataDriven]
	public class ApplicationVersionServiceTests : WebServiceTestsBase
	{
		[IdentifiedTest("d54cce55-4d67-4488-8d1f-2e2f40d7bf28")]
		[Feature.DeveloperPlatform.ExtensibilityPoints.Api.WebApi]
		[Category(TestCategories.NotInCompatibility)]
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

		[IdentifiedTest("4a236132-09c9-4767-968e-cb15ea2ee5c2")]
		[Feature.DeveloperPlatform.ExtensibilityPoints.Api.RestApi]
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