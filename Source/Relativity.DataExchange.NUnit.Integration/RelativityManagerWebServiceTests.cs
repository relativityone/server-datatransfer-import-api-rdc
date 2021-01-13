// -----------------------------------------------------------------------------------------------------
// <copyright file="RelativityManagerWebServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="kCura.WinEDDS.Service.RelativityManager"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Integration
{
	using System;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;

	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	/// <summary>
	/// Represents <see cref="RelativityManager"/> tests.
	/// </summary>
	[TestFixture]
	[Feature.DeveloperPlatform.ExtensibilityPoints.Api.WebApi]
	[TestType.MainFlow]
	public class RelativityManagerWebServiceTests : WebServiceTestsBase
	{
		[IdentifiedTest("c0c2a5b9-4149-4dc2-9cb3-df6de3612804")]
		[Category(TestCategories.NotInCompatibility)]
		public void ShouldReadTheCaseInfo()
		{
			using (RelativityManager relativityManager = new RelativityManager(
				this.RelativityInstance.Credentials,
				this.RelativityInstance.CookieContainer,
				this.RelativityInstance.WebApiServiceUrl.ToString()))
			{
				string returnedValue = relativityManager.GetImportExportWebApiVersion();

				Version webApiVer = Version.Parse(returnedValue);

				Assert.That(webApiVer, Is.Not.Null);
				Assert.That(webApiVer, Is.GreaterThanOrEqualTo(Version.Parse("1.0")));
			}
		}
	}
}
