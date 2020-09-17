// <copyright file="RelativityDistributedFacadeRetriesDecoratorTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service.RelativityDistributed
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Service.RelativityDistributed;
	using Relativity.Logging;

	[TestFixture]
	public class RelativityDistributedFacadeRetriesDecoratorTests
	{
		private IRelativityDistributedFacade sut;

		private Mock<ILog> loggerMock;

		private Mock<IAppSettings> settingsMock;

		private Mock<IRelativityDistributedFacade> decoratedFacadeMock;

		[SetUp]
		public void SetUp()
		{
			this.loggerMock = new Mock<ILog>();
			this.settingsMock = new Mock<IAppSettings>();
			this.decoratedFacadeMock = new Mock<IRelativityDistributedFacade>();

			this.settingsMock
				.Setup(x => x.HttpErrorWaitTimeInSeconds)
				.Returns(0); // we don't want to wait in unit tests
			this.settingsMock
				.Setup(x => x.HttpErrorNumberOfRetries)
				.Returns(3);

			this.sut = new RelativityDistributedFacadeRetriesDecorator(
				this.loggerMock.Object,
				this.settingsMock.Object,
				this.decoratedFacadeMock.Object);
		}

		[Test]
		public void ShouldReturnSuccessfulResponseWhenFirstCallWasSuccessful()
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var expectedResponse = new FileDownloadResponse();
			this.decoratedFacadeMock.Setup(x => x.DownloadFile(downloadRequest)).Returns(expectedResponse);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(
				actualResponse.IsSuccess,
				Is.True,
				"It should succeed, because decorated facade returned successful response");
			Assert.That(actualResponse, Is.EqualTo(expectedResponse));

			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Once);
		}

		[Test]
		public void ShouldReturnUnsuccessfulResponseWhenFirstCallFailedAndErrorWasNonRetryable()
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var expectedResponse = new FileDownloadResponse(RelativityDistributedErrorType.NotFound);
			this.decoratedFacadeMock.Setup(x => x.DownloadFile(downloadRequest)).Returns(expectedResponse);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(
				actualResponse.IsSuccess,
				Is.False,
				"It should fail, because decorated facade returned error which cannot be retried");
			Assert.That(actualResponse, Is.EqualTo(expectedResponse));

			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Once);
		}

		[Test]
		public void ShouldReturnSuccessfulResponseWhenSecondCallSucceeded()
		{
			RelativityDistributedErrorType[] retryableErrors =
				{
					RelativityDistributedErrorType.InternalServerError,
					RelativityDistributedErrorType.DataCorrupted
				};

			foreach (var retryableError in retryableErrors)
			{
				this.ShouldReturnSuccessfulResponseWhenSecondCallSucceeded(retryableError);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "1+maxNumberOfRetries", Justification = "It is not an issue here.")]
		[TestCase(2)]
		[TestCase(5)]
		public void ShouldReadMaximumNumberOfRetriesFromSettings(int maxNumberOfRetries)
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var failedResponse = new FileDownloadResponse(RelativityDistributedErrorType.InternalServerError);
			this.decoratedFacadeMock
				.Setup(x => x.DownloadFile(downloadRequest))
				.Returns(failedResponse);
			this.settingsMock.Setup(x => x.HttpErrorNumberOfRetries).Returns(maxNumberOfRetries);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(actualResponse.IsSuccess, Is.False, "It should fail, because all attempts failed.");
			Assert.That(actualResponse, Is.EqualTo(failedResponse));

			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Exactly(1 + maxNumberOfRetries));
		}

		internal void ShouldReturnSuccessfulResponseWhenSecondCallSucceeded(RelativityDistributedErrorType errorTypeForFirstCall)
		{
			// arrange
			var downloadRequest = new FileDownloadRequest("path", "workspaceId", "guid");
			var firstResponse = new FileDownloadResponse(errorTypeForFirstCall);
			var secondResponse = new FileDownloadResponse();
			this.decoratedFacadeMock
				.SetupSequence(x => x.DownloadFile(downloadRequest))
				.Returns(firstResponse)
				.Returns(secondResponse);

			// act
			var actualResponse = this.sut.DownloadFile(downloadRequest);

			// assert
			Assert.That(
				actualResponse.IsSuccess,
				Is.True,
				"It should succeed, because second call to decorated facade was successful.");
			Assert.That(actualResponse, Is.EqualTo(secondResponse));

			this.decoratedFacadeMock.Verify(x => x.DownloadFile(downloadRequest), Times.Exactly(2));
		}
	}
}
