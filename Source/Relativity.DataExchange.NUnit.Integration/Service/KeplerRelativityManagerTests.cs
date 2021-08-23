// <copyright file="KeplerRelativityManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	using Relativity.Testing.Identification;

	[TestFixture(true)]
	[TestFixture(false)]
	[Feature.DataTransfer.ImportApi]
	public class KeplerRelativityManagerTests : KeplerServiceTestBase
	{
		public KeplerRelativityManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[IdentifiedTest("7f156cab-8c20-45e9-8721-bd301bc3bffe")]
		public void ShouldReturnFalseForCheckIfEmailNotificationEnabled()
		{
			// arrange
			using (IRelativityManager sut = ManagerFactory.CreateRelativityManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(sut.IsImportEmailNotificationEnabled(), Is.False);
			}
		}

		[IdentifiedTest("35aae7c9-0eeb-4625-8d73-451415e39332")]
		public void ShouldReturnCurrencySymbol()
		{
			// arrange
			using (IRelativityManager sut = ManagerFactory.CreateRelativityManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(sut.RetrieveCurrencySymbol(), Is.EqualTo("$"));
			}
		}

		[IdentifiedTest("978c6af5-0d81-4c7e-b0ec-2f840f96a7d6")]
		public void ShouldReturnFalseForValidateSuccessfulLogForValidCredentials()
		{
			// arrange
			using (IRelativityManager sut = ManagerFactory.CreateRelativityManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(sut.ValidateSuccessfulLogin(), Is.EqualTo(false));
			}
		}

		[IdentifiedTest("d7a0725c-3e4e-430c-b77b-7557d1f7f672")]
		public void ShouldReturnFalseForValidateSuccessfulLogForInvalidCredentials()
		{
			// Arrange
			this.Credential.UserName = "INVALID USERNAME";
			this.Credential.Password = "INVALID PASSWORD";

			using (IRelativityManager sut = ManagerFactory.CreateRelativityManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act & assert
				Assert.That(sut.ValidateSuccessfulLogin(), Is.EqualTo(false));
			}
		}

		[IdentifiedTest("65eb76d6-702b-4ead-bcfd-ec3d76efc37d")]
		public void ShouldReturnCorrectRdcConfiguration()
		{
			// arrange
			using (IRelativityManager sut = ManagerFactory.CreateRelativityManager(
				this.Credential,
				this.CookieContainer,
				this.CorrelationIdFunc))
			{
				// act
				var rdcConfiguration = sut.RetrieveRdcConfiguration();

				// assert
				Assert.That(rdcConfiguration, Is.Not.Null);
				var table = rdcConfiguration.Tables[0];
				Assert.That(table.Columns.Count, Is.EqualTo(3));
				Assert.That(table.Columns.IndexOf("Section"), Is.EqualTo(0));
				Assert.That(table.Columns.IndexOf("Name"), Is.EqualTo(1));
				Assert.That(table.Columns.IndexOf("Value"), Is.EqualTo(2));
				Assert.That(table.Rows.Count, Is.GreaterThan(0));
			}
		}
	}
}
