// <copyright file="KeplerRelativityManagerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration.Service
{
	using global::NUnit.Framework;

	using kCura.WinEDDS.Service;
	using kCura.WinEDDS.Service.Replacement;

	[TestFixture(true)]
	[TestFixture(false)]
	public class KeplerRelativityManagerTests : KeplerServiceTestBase
	{
		public KeplerRelativityManagerTests(bool useKepler)
			: base(useKepler)
		{
		}

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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

		[Test]
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
