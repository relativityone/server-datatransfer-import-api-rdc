// <copyright file="WebApiVsKeplerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;
	using Relativity.Services.Exceptions;

	[TestFixture]
	public class WebApiVsKeplerTests
	{
		private Mock<IServiceAvailabilityChecker> serviceAvailabilityCheckerMock;
		private WebApiVsKepler webApiVsKepler;

		[SetUp]
		public void SetUp()
		{
			this.serviceAvailabilityCheckerMock = new Mock<IServiceAvailabilityChecker>();
			this.webApiVsKepler = new WebApiVsKepler(this.serviceAvailabilityCheckerMock.Object);
		}

		[TestCase(true, true, IAPICommunicationMode.Kepler, null, true)]
		[TestCase(true, true, IAPICommunicationMode.Kepler, true, true)]
		[TestCase(true, true, IAPICommunicationMode.Kepler, false, false)]
		[TestCase(true, true, IAPICommunicationMode.WebAPI, null, false)]
		[TestCase(true, true, IAPICommunicationMode.WebAPI, true, true)]
		[TestCase(true, true, IAPICommunicationMode.WebAPI, false, false)]
		[TestCase(true, true, IAPICommunicationMode.ForceKepler, null, true)]
		[TestCase(true, true, IAPICommunicationMode.ForceKepler, true, true)]
		[TestCase(true, true, IAPICommunicationMode.ForceKepler, false, true)]
		[TestCase(true, true, IAPICommunicationMode.ForceWebAPI, null, false)]
		[TestCase(true, true, IAPICommunicationMode.ForceWebAPI, true, false)]
		[TestCase(true, true, IAPICommunicationMode.ForceWebAPI, false, false)]
		[TestCase(true, false, IAPICommunicationMode.Kepler, null, false)]
		[TestCase(true, false, IAPICommunicationMode.Kepler, true, false)]
		[TestCase(true, false, IAPICommunicationMode.Kepler, false, false)]
		[TestCase(true, false, IAPICommunicationMode.WebAPI, null, false)]
		[TestCase(true, false, IAPICommunicationMode.WebAPI, true, false)]
		[TestCase(true, false, IAPICommunicationMode.WebAPI, false, false)]
		[TestCase(true, false, IAPICommunicationMode.ForceKepler, null, false)]
		[TestCase(true, false, IAPICommunicationMode.ForceKepler, true, false)]
		[TestCase(true, false, IAPICommunicationMode.ForceKepler, false, false)]
		[TestCase(true, false, IAPICommunicationMode.ForceWebAPI, null, false)]
		[TestCase(true, false, IAPICommunicationMode.ForceWebAPI, true, false)]
		[TestCase(true, false, IAPICommunicationMode.ForceWebAPI, false, false)]
		[TestCase(false, true, IAPICommunicationMode.Kepler, null, true)]
		[TestCase(false, true, IAPICommunicationMode.Kepler, true, true)]
		[TestCase(false, true, IAPICommunicationMode.Kepler, false, true)]
		[TestCase(false, true, IAPICommunicationMode.WebAPI, null, true)]
		[TestCase(false, true, IAPICommunicationMode.WebAPI, true, true)]
		[TestCase(false, true, IAPICommunicationMode.WebAPI, false, true)]
		[TestCase(false, true, IAPICommunicationMode.ForceKepler, null, true)]
		[TestCase(false, true, IAPICommunicationMode.ForceKepler, true, true)]
		[TestCase(false, true, IAPICommunicationMode.ForceKepler, false, true)]
		[TestCase(false, true, IAPICommunicationMode.ForceWebAPI, null, true)]
		[TestCase(false, true, IAPICommunicationMode.ForceWebAPI, true, true)]
		[TestCase(false, true, IAPICommunicationMode.ForceWebAPI, false, true)]
		public void UseKeplerShouldReturnCorrectValue(
			bool isWebApiAvailable,
			bool isKeplerAvailable,
			IAPICommunicationMode iApiCommunicationMode,
			bool? useKeplerAppSettingsValue,
			bool expectedResult)
		{
			// arrange
			this.serviceAvailabilityCheckerMock.Setup(m => m.IsWebApiAvailable()).Returns(isWebApiAvailable);
			this.serviceAvailabilityCheckerMock.Setup(m => m.IsKeplerAvailable()).Returns(isKeplerAvailable);
			this.serviceAvailabilityCheckerMock.Setup(m => m.ReadImportApiCommunicationMode()).Returns(iApiCommunicationMode);
			AppSettings.Instance.UseKepler = useKeplerAppSettingsValue;

			// act
			var actualResult = this.webApiVsKepler.UseKepler();

			// assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[TestCase(IAPICommunicationMode.Kepler, null)]
		[TestCase(IAPICommunicationMode.Kepler, true)]
		[TestCase(IAPICommunicationMode.Kepler, false)]
		[TestCase(IAPICommunicationMode.WebAPI, null)]
		[TestCase(IAPICommunicationMode.WebAPI, true)]
		[TestCase(IAPICommunicationMode.WebAPI, false)]
		[TestCase(IAPICommunicationMode.ForceKepler, null)]
		[TestCase(IAPICommunicationMode.ForceKepler, true)]
		[TestCase(IAPICommunicationMode.ForceKepler, false)]
		[TestCase(IAPICommunicationMode.ForceWebAPI, null)]
		[TestCase(IAPICommunicationMode.ForceWebAPI, true)]
		[TestCase(IAPICommunicationMode.ForceWebAPI, false)]
		public void UseKeplerAndKeplerNotAvailableAndWebApiNotAvailableShouldThrowException(
			IAPICommunicationMode iApiCommunicationMode,
			bool? useKeplerAppSettingsValue)
		{
			// arrange
			this.serviceAvailabilityCheckerMock.Setup(m => m.IsWebApiAvailable()).Returns(false);
			this.serviceAvailabilityCheckerMock.Setup(m => m.IsKeplerAvailable()).Returns(false);
			this.serviceAvailabilityCheckerMock.Setup(m => m.ReadImportApiCommunicationMode()).Returns(iApiCommunicationMode);
			AppSettings.Instance.UseKepler = useKeplerAppSettingsValue;

			// act & assert
			var error = Assert.Throws<NotFoundException>(() => this.webApiVsKepler.UseKepler());
			Assert.True(error.Message.Contains("The request could not be executed because the Import API service endpoint could not be found. " +
				"A system administrator can attempt to resolve this issue by " +
				"1) Installing the web certificates to the Trusted Root store " +
				"2) Add the web server to Internet Options -> Trusted Sites, and " +
				"3) Verify the WebAPI URL is configured to the Web server. If the problem persists, Restart the \"kCura Service Host Manager\" and " +
				"\"kCura Agent manager\" Windows services or contact your system administrator for assistance."));
		}

		[Test]
		public void UseKeplerAndCommunicationModeNullShouldReturnTrue()
		{
			// arrange
			this.serviceAvailabilityCheckerMock.Setup(m => m.IsWebApiAvailable()).Returns(true);
			this.serviceAvailabilityCheckerMock.Setup(m => m.IsKeplerAvailable()).Returns(true);
			AppSettings.Instance.UseKepler = null;

			// act & assert
			var result = this.webApiVsKepler.UseKepler();
			Assert.True(result);
		}
	}
}
