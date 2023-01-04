// <copyright file="RelativityDistributedFacadeAuthenticationDecoratorTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service.RelativityDistributed
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.RelativityDistributed;
	using Relativity.Logging;

	[TestFixture]
	public class RelativityDistributedFacadeAuthenticationDecoratorTests
	{
		private IRelativityDistributedFacade sut;

		private Mock<ILog> loggerMock;
		private Mock<IAppSettings> settingsMock;
		private Mock<IReLoginService> reLoginServiceMock;
		private Mock<IRelativityDistributedFacade> decoratedFacadeMock;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILog>();
			this.settingsMock = new Mock<IAppSettings>();
			this.reLoginServiceMock = new Mock<IReLoginService>();
			this.decoratedFacadeMock = new Mock<IRelativityDistributedFacade>();

			this.sut = new RelativityDistributedFacadeAuthenticationDecorator(
				this.loggerMock.Object,
				this.settingsMock.Object,
				this.reLoginServiceMock.Object,
				this.decoratedFacadeMock.Object);
		}

		[Test]
		public void ShouldReturnSuccessfulResponseWhenFirstCallSuccessful()
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var expectedResponse = new FileDownloadResponse();
			this.decoratedFacadeMock
				.Setup(x => x.DownloadFile(downloadRequest))
				.Returns(expectedResponse);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(actualResponse.IsSuccess, Is.True, "It should succeed, because decorated facade returned successful response");
			Assert.That(actualResponse, Is.EqualTo(expectedResponse));

			this.reLoginServiceMock.Verify(x => x.AttemptReLogin(true), Times.Never);
			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Once);
		}

		[Test]
		public void ShouldCallReloginServiceWhenFirstRequestWasUnsuccessful()
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var firstResponse = new FileDownloadResponse(RelativityDistributedErrorType.Authentication);
			var secondResponse = new FileDownloadResponse();
			this.decoratedFacadeMock
				.SetupSequence(x => x.DownloadFile(downloadRequest))
				.Returns(firstResponse)
				.Returns(secondResponse);
			this.settingsMock.Setup(x => x.MaxReloginTries).Returns(1);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(actualResponse.IsSuccess, Is.True, "It should succeed, because second attempt was successful.");
			Assert.That(actualResponse, Is.EqualTo(secondResponse));

			this.reLoginServiceMock.Verify(x => x.AttemptReLogin(true), Times.Once);
			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Exactly(2));
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "1+maxNumberOfRetries", Justification = "It is not an issue here.")]
		[TestCase(2)]
		[TestCase(5)]
		public void ShouldReadMaximumNumberOfRetriesFromSettings(int maxNumberOfRetries)
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var failedResponse = new FileDownloadResponse(RelativityDistributedErrorType.Authentication);
			this.decoratedFacadeMock
				.Setup(x => x.DownloadFile(downloadRequest))
				.Returns(failedResponse);
			this.settingsMock.Setup(x => x.MaxReloginTries).Returns(maxNumberOfRetries);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(actualResponse.IsSuccess, Is.False, "It should fail, because all attempts failed.");
			Assert.That(actualResponse, Is.EqualTo(failedResponse));

			this.reLoginServiceMock.Verify(x => x.AttemptReLogin(true), Times.Exactly(maxNumberOfRetries));
			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Exactly(1 + maxNumberOfRetries));
		}
	}
}
