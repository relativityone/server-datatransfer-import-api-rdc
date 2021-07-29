// <copyright file="ServiceAvailabilityCheckerTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service
{
	using System;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.NUnit.Mocks;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.Services.Exceptions;

	[TestFixture]
	public class ServiceAvailabilityCheckerTests
	{
		private Mock<IIAPICommunicationModeService> iApiCommunicationModeServiceMock;
		private Mock<IServiceProxyFactory> serviceProxyFactoryMock;
		private IIAPICommunicationModeManager iApiCommunicationModeManager;
		private IServiceAvailabilityChecker serviceAvailabilityChecker;

		[SetUp]
		public void SetUp()
		{
			this.iApiCommunicationModeServiceMock = new Mock<IIAPICommunicationModeService>();

			this.serviceProxyFactoryMock = new Mock<IServiceProxyFactory>();
			this.serviceProxyFactoryMock
				.Setup(m => m.CreateProxyInstance<IIAPICommunicationModeService>())
				.Returns(this.iApiCommunicationModeServiceMock.Object);
			var keplerProxy = new KeplerProxyMock(this.serviceProxyFactoryMock.Object);
			this.iApiCommunicationModeManager = new IAPICommunicationModeManager(keplerProxy, null);
			this.serviceAvailabilityChecker = new ServiceAvailabilityChecker(this.iApiCommunicationModeManager);
		}

		[Test]
		public void IsKeplerAvailableAndNotAuthorizedExceptionThrownShouldReturnTrue()
		{
			// arrange
			this.iApiCommunicationModeServiceMock
				.Setup(m => m.GetIAPICommunicationModeAsync(It.IsAny<string>()))
				.Throws<NotAuthorizedException>();

			// act
			var isKeplerAvailable = this.serviceAvailabilityChecker.IsKeplerAvailable();

			// assert
			Assert.True(isKeplerAvailable);
		}

		[Test]
		public void IsKeplerAvailableAndExceptionThrownShouldReturnFalse()
		{
			// arrange
			this.iApiCommunicationModeServiceMock
				.Setup(m => m.GetIAPICommunicationModeAsync(It.IsAny<string>()))
				.Throws<Exception>();

			// act
			var isKeplerAvailable = this.serviceAvailabilityChecker.IsKeplerAvailable();

			// assert
			Assert.False(isKeplerAvailable);
		}
	}
}
