// ----------------------------------------------------------------------------
// <copyright file="IAPICommunicationModeManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.NUnit.Mocks;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1.Models;
	using Relativity.Logging;

	[TestFixture]
	public class IAPICommunicationModeManagerTests
	{
		private IIAPICommunicationModeManager _sut;

		private KeplerProxyMock _keplerProxyMock;
		private Mock<IAppSettings> _appSettingsMock;
		private Mock<ILog> _logMock;

		private Mock<IServiceProxyFactory> _iServiceProxyFactory;

		[SetUp]
		public void SetUp()
		{
			this._appSettingsMock = new Mock<IAppSettings>();
			this._iServiceProxyFactory = new Mock<IServiceProxyFactory>();
			this._keplerProxyMock = new KeplerProxyMock(this._iServiceProxyFactory.Object);
			this._logMock = new Mock<ILog>();
			this._sut = new IAPICommunicationModeManager(this._keplerProxyMock, this.CorrelationIdFuncMock, this._appSettingsMock.Object, this._logMock.Object);
		}

		[TestCase("\"12.2.0\"", IAPICommunicationMode.Kepler)]
		[TestCase("\"12.3.0\"", IAPICommunicationMode.Kepler)]
		[TestCase("\"12.3.0\"", IAPICommunicationMode.WebAPI)]
		[TestCase("\"12.3.0\"", IAPICommunicationMode.ForceWebAPI)]
		[TestCase("\"12.3.0\"", IAPICommunicationMode.ForceKepler)]
		[Test]
		public async Task ReadImportApiCommunicationMode(string currentVersion, IAPICommunicationMode expectedResult)
		{
			// Arrange
			Mock<IIAPICommunicationModeService> iAPICommunicationModeServiceMock = new Mock<IIAPICommunicationModeService>();
			iAPICommunicationModeServiceMock.Setup(m => m.GetIAPICommunicationModeAsync(It.IsAny<string>())).ReturnsAsync(expectedResult);
			this._iServiceProxyFactory.Setup(m => m.ExecutePostAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(currentVersion);
			this._iServiceProxyFactory.Setup(m => m.CreateProxyInstance<IIAPICommunicationModeService>()).Returns(iAPICommunicationModeServiceMock.Object);

			// Act
			IAPICommunicationMode result = await this._sut.ReadImportApiCommunicationMode().ConfigureAwait(false);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[TestCase("\"12.0.0\"", IAPICommunicationMode.ForceWebAPI)]
		[Test]
		public async Task ReadImportApiCommunicationModeShouldReturnForceWebAPI(string currentVersion, IAPICommunicationMode expectedResult)
		{
			// Arrange
			this._iServiceProxyFactory.Setup(m => m.ExecutePostAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(currentVersion);

			// Act
			IAPICommunicationMode result = await this._sut.ReadImportApiCommunicationMode().ConfigureAwait(false);

			// Assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public void ReadImportApiCommunicationModeShouldThrowExceptionCallingForVersion()
		{
			// Arrange
			Mock<IIAPICommunicationModeService> iAPICommunicationModeServiceMock = new Mock<IIAPICommunicationModeService>();
			iAPICommunicationModeServiceMock.Setup(m => m.GetIAPICommunicationModeAsync(It.IsAny<string>())).Throws<Exception>();
			this._iServiceProxyFactory.Setup(m => m.ExecutePostAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
			this._iServiceProxyFactory.Setup(m => m.CreateProxyInstance<IIAPICommunicationModeService>()).Returns(iAPICommunicationModeServiceMock.Object);

			// Act && Assert
			Assert.CatchAsync(() => this._sut.ReadImportApiCommunicationMode());
		}

		[Test]
		public void ReadImportApiCommunicationModeShouldThrowException()
		{
			// Arrange
			Mock<IIAPICommunicationModeService> iAPICommunicationModeServiceMock = new Mock<IIAPICommunicationModeService>();
			iAPICommunicationModeServiceMock.Setup(m => m.GetIAPICommunicationModeAsync(It.IsAny<string>())).Throws<Exception>();
			this._iServiceProxyFactory.Setup(m => m.ExecutePostAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("\"12.3.0\"");
			this._iServiceProxyFactory.Setup(m => m.CreateProxyInstance<IIAPICommunicationModeService>()).Returns(iAPICommunicationModeServiceMock.Object);

			// Act && Assert
			Assert.CatchAsync(() => this._sut.ReadImportApiCommunicationMode());
		}

		[Test]
		public async Task ReadImportApiCommunicationModeShouldReturnModeWhenPartialTasklyFailed([Values(IAPICommunicationMode.ForceWebAPI, IAPICommunicationMode.WebAPI, IAPICommunicationMode.Kepler, IAPICommunicationMode.ForceKepler)]IAPICommunicationMode value)
		{
			// Arrange
			Mock<IIAPICommunicationModeService> iAPICommunicationModeServiceMock = new Mock<IIAPICommunicationModeService>();
			iAPICommunicationModeServiceMock.Setup(m => m.GetIAPICommunicationModeAsync(It.IsAny<string>())).ReturnsAsync(value);
			this._iServiceProxyFactory.Setup(m => m.ExecutePostAsync(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
			this._iServiceProxyFactory.Setup(m => m.CreateProxyInstance<IIAPICommunicationModeService>()).Returns(iAPICommunicationModeServiceMock.Object);

			// Act
			IAPICommunicationMode result = await this._sut.ReadImportApiCommunicationMode().ConfigureAwait(false);

			// Assert
			Assert.That(result, Is.EqualTo(value));
		}

		private string CorrelationIdFuncMock()
		{
			return string.Empty;
		}
	}
}