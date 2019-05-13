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
	[Feature.DataTransfer.ImportApi]
	public class RelativityManagerWebServiceTests : WebServiceTestsBase
	{
		[Test]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.WebService)]
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
