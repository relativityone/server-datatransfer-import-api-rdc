// <copyright file="KeplerProxyTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.NUnit.Service
{
	using System;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Credentials;
	using kCura.WinEDDS.Service.Kepler;

	using Moq;

	using Relativity.DataExchange.Service;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;

	[TestFixture]
	public class KeplerProxyTests
	{
		private Mock<IServiceProxyFactory> serviceFactoryMock;
		private Mock<ILog> loggerMock;
		private KeplerProxy sut;

		[SetUp]
		public void SetUp()
		{
			this.serviceFactoryMock = new Mock<IServiceProxyFactory>();
			this.loggerMock = new Mock<ILog>();

			AppSettings.Instance.MaxReloginTries = 1;

			this.sut = new KeplerProxy(this.serviceFactoryMock.Object, this.loggerMock.Object);
		}

		[Test]
		public async Task ShouldReturnResultOfFunctionAsync()
		{
			// arrange
			string expectedResult = "result";
			Task<string> Func(IServiceProxyFactory serviceProxyFactory)
			{
				return Task.FromResult(expectedResult);
			}

			// act
			string result = await this.sut.ExecuteAsync(Func).ConfigureAwait(false);

			// assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task ShouldRetryOnNotAuthorizedExceptionForTaskOfResultAsync()
		{
			// arrange
			string expectedResult = "result";
			bool shouldThrowNotAuthorizedException = true;
			Task<string> Func(IServiceProxyFactory serviceProxyFactory)
			{
				if (shouldThrowNotAuthorizedException)
				{
					shouldThrowNotAuthorizedException = false;
					throw new NotAuthorizedException();
				}

				return Task.FromResult(expectedResult);
			}

			// act
			string result = await this.sut.ExecuteAsync(Func).ConfigureAwait(false);

			// assert
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task ShouldRetryOnNotAuthorizedExceptionForTaskAsync()
		{
			// arrange
			bool shouldThrowNotAuthorizedException = true;
			Task Func(IServiceProxyFactory serviceProxyFactory)
			{
				if (shouldThrowNotAuthorizedException)
				{
					shouldThrowNotAuthorizedException = false;
					throw new NotAuthorizedException();
				}

				return Task.CompletedTask;
			}

			// act
			await this.sut.ExecuteAsync(Func).ConfigureAwait(false);

			// assert
			Assert.Pass("It should not thrown an exception.");
		}

		[Test]
		public void ShouldNotRetryIndefinitelyOnNotAuthorizedExceptionForTaskOfResult()
		{
			// arrange
			AppSettings.Instance.MaxReloginTries = 2;
			const int ExpectedNumberOfRetries = 1;
			const int ExpectedNumberOfCalls = ExpectedNumberOfRetries + 1;

			int numberOfCalls = 0;
			Task<string> Func(IServiceProxyFactory serviceProxyFactory)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act @ assert
			Assert.That(() => this.sut.ExecuteAsync(Func), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(ExpectedNumberOfCalls));
		}

		[Test]
		public void ShouldNotRetryIndefinitelyOnNotAuthorizedExceptionForTask()
		{
			// arrange
			AppSettings.Instance.MaxReloginTries = 2;
			const int ExpectedNumberOfRetries = 1;
			const int ExpectedNumberOfCalls = ExpectedNumberOfRetries + 1;

			int numberOfCalls = 0;
			Task Func(IServiceProxyFactory serviceProxyFactory)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act @ assert
			Assert.That(() => this.sut.ExecuteAsync(Func), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(ExpectedNumberOfCalls));
		}

		[Test]
		public void ShouldUseCredentialsProviderAndUpdateCredentialsOnNotAuthorizedException()
		{
			// arrange
			const int NumberOfRetries = 2;
			const int ExpectedNumberOfCalls = NumberOfRetries + 1;
			var updatedCredentials = new NetworkCredential("username", "password");
			RelativityWebApiCredentialsProvider.Instance().SetProvider(new TokenProvider(updatedCredentials));
			AppSettings.Instance.MaxReloginTries = NumberOfRetries;

			int numberOfCalls = 0;
			Task<string> Func(IServiceProxyFactory serviceProxyFactory)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act & assert
			Assert.That(() => this.sut.ExecuteAsync(Func), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(ExpectedNumberOfCalls), "It should retry defined number of times, when credentials provider is registered.");
			this.serviceFactoryMock.Verify(x => x.UpdateCredentials(updatedCredentials), Times.Exactly(NumberOfRetries));
		}

		[Test]
		public void ShouldNotRetryTwiceOnNotAuthorizedExceptionWhenCredentialsProviderNotRegistered()
		{
			// arrange
			RelativityWebApiCredentialsProvider.Instance().SetProvider(null);
			AppSettings.Instance.MaxReloginTries = 2;

			int numberOfCalls = 0;
			Task<string> Func(IServiceProxyFactory serviceProxyFactory)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act & assert
			Assert.That(() => this.sut.ExecuteAsync(Func), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(2), "It should do only one retry when credentials provider is not registered.");
			this.serviceFactoryMock.Verify(x => x.UpdateCredentials(It.IsAny<NetworkCredential>()), Times.Never);
		}

		private class TokenProvider : ICredentialsProvider
		{
			private readonly NetworkCredential credential;

			public TokenProvider(NetworkCredential credential)
			{
				this.credential = credential;
			}

			public NetworkCredential GetCredentials()
			{
				return this.credential;
			}

			public Task<NetworkCredential> GetCredentialsAsync(CancellationToken cancellationToken)
			{
				return Task.FromResult(this.GetCredentials());
			}
		}
	}
}
