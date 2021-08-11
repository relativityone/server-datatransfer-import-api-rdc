// <copyright file="CertificateValidatorTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Integration
{
	using System.Net;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;
	using Relativity.Testing.Identification;

	[TestFixture]
	public class CertificateValidatorTests
	{
		private Mock<IAppSettings> appSettingsMock;
		private Mock<ILog> loggerMock;

		private CertificateValidator sut;

		private string originalWebApiServiceUrl;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			this.originalWebApiServiceUrl = AppSettings.Instance.ProgrammaticWebApiServiceUrl;
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = this.originalWebApiServiceUrl;
		}

		[SetUp]
		public void SetUp()
		{
			this.appSettingsMock = new Mock<IAppSettings>();
			this.loggerMock = new Mock<ILog>();
			var cookieContainer = new CookieContainer();

			this.sut = new CertificateValidator(
				this.appSettingsMock.Object,
				cookieContainer,
				this.loggerMock.Object,
				correlationIdProvider: () => "CorrelationId");
		}

		[IdentifiedTestCase("6bb2bd15-5341-4bee-a463-d7f06b4dff4d", true)]
		[IdentifiedTestCase("c6ab6719-a701-4140-a932-5c7c793c2b98", false)]
		public void ShouldValidateCertificate(bool useKepler)
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = IntegrationTestHelper.IntegrationTestParameters.RelativityWebApiUrl.ToString();
			this.appSettingsMock.Setup(x => x.UseKepler).Returns(useKepler);

			// act
			bool isCertificateTrusted = this.sut.IsCertificateTrusted();

			// assert
			Assert.That(isCertificateTrusted, Is.True, "Certificate is trusted");
		}

		[IdentifiedTestCase("18d756c5-6c3b-49da-b7b2-55a58feacf7e", true)]
		[IdentifiedTestCase("aa3fefe6-1104-4f7b-9b4e-1b0f2019dae5", false)]
		public void ShouldThrowExceptionWhenUrlIsInvalid(bool useKepler)
		{
			// arrange
			AppSettings.Instance.ProgrammaticWebApiServiceUrl = "https://invalidUrl.relativity.com";
			this.appSettingsMock.Setup(x => x.UseKepler).Returns(useKepler);

			// act & assert
			Assert.That(
				() => this.sut.IsCertificateTrusted(),
				Throws.Exception
					.TypeOf<WebException>()
					.With.Message.StartsWith("The remote name could not be resolved:"));
		}
	}
}
