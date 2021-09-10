// <copyright file="ReLoginRetryPolicyFactoryTests.cs" company="Relativity ODA LLC">
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

	using Polly;

	using Relativity.DataExchange.Service;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "ReLogin is a common phrase in RDC")]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	[TestFixture(true)]
	[TestFixture(false)]
	public class ReLoginRetryPolicyFactoryTests
	{
		private const string RetryCountKey = "TestRetryCount";
		private const int MaxRetryCount = 2;

		private readonly bool useCustomOnRetry;

		private Mock<IAppSettings> settings;
		private Mock<ILog> logger;
		private IAsyncPolicy policy;
		private CancellationTokenSource cancellationTokenSource;
		private Context context;
		private Mock<IServiceProxyFactory> serviceProxyFactoryMock;

		public ReLoginRetryPolicyFactoryTests(bool useCustomOnRetry)
		{
			this.useCustomOnRetry = useCustomOnRetry;
		}

		[SetUp]
		public void Setup()
		{
			RelativityWebApiCredentialsProvider.Instance().SetProvider(null);

			this.cancellationTokenSource = new CancellationTokenSource();
			this.context = new Context("test");
			this.settings = new Mock<IAppSettings>();
			this.logger = new Mock<ILog>();
			this.serviceProxyFactoryMock = new Mock<IServiceProxyFactory>();

			this.settings.SetupGet(x => x.MaxReloginTries).Returns(MaxRetryCount);
			this.policy = this.useCustomOnRetry ?
				              new ReLoginRetryPolicyFactory(this.settings.Object, this.serviceProxyFactoryMock.Object, this.logger.Object, (exception, duration, retryCount, ctx) => { ctx[RetryCountKey] = retryCount; }).CreateRetryPolicy() :
				              new ReLoginRetryPolicyFactory(this.settings.Object, this.serviceProxyFactoryMock.Object, this.logger.Object).CreateRetryPolicy();
		}

		[TearDown]
		public void TearDown()
		{
			RelativityWebApiCredentialsProvider.Instance().SetProvider(null);
			this.cancellationTokenSource?.Dispose();
		}

		[Test]
		public async Task ShouldReturnResultOfFunctionAsync()
		{
			// arrange
			const int ExpectedNumberOfRetries = 0;
			string expectedResult = "result";
			Task<string> Func(Context context, CancellationToken cancellationToken)
			{
				return Task.FromResult(expectedResult);
			}

			// act
			string result = await this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token).ConfigureAwait(false);

			// assert
			this.ThenOnRetryWasExecuted(ExpectedNumberOfRetries);
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task ShouldRetryOnNotAuthorizedExceptionForTaskOfResultAsync()
		{
			// arrange
			const int ExpectedNumberOfRetries = 1;
			string expectedResult = "result";
			bool shouldThrowNotAuthorizedException = true;
			Task<string> Func(Context context, CancellationToken cancellationToken)
			{
				if (shouldThrowNotAuthorizedException)
				{
					shouldThrowNotAuthorizedException = false;
					throw new NotAuthorizedException();
				}

				return Task.FromResult(expectedResult);
			}

			// act
			string result = await this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token).ConfigureAwait(false);

			// assert
			this.ThenOnRetryWasExecuted(ExpectedNumberOfRetries);
			Assert.That(result, Is.EqualTo(expectedResult));
		}

		[Test]
		public async Task ShouldRetryOnNotAuthorizedExceptionForTaskAsync()
		{
			// arrange
			const int ExpectedNumberOfRetries = 1;
			bool shouldThrowNotAuthorizedException = true;

			Task Func(Context context, CancellationToken cancellationToken)
			{
				if (shouldThrowNotAuthorizedException)
				{
					shouldThrowNotAuthorizedException = false;
					throw new NotAuthorizedException();
				}

				return Task.CompletedTask;
			}

			// act
			await this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token).ConfigureAwait(false);

			// assert
			this.ThenOnRetryWasExecuted(ExpectedNumberOfRetries);
			Assert.Pass("It should not thrown an exception.");
		}

		[Test]
		public void ShouldNotRetryIndefinitelyOnNotAuthorizedExceptionForTaskOfResult()
		{
			// arrange
			const int ExpectedNumberOfRetries = 1;
			const int ExpectedNumberOfCalls = ExpectedNumberOfRetries + 1;

			int numberOfCalls = 0;
			Task<string> Func(Context context, CancellationToken cancellationToken)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act @ assert
			Assert.That(() => this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(ExpectedNumberOfCalls));
			this.ThenOnRetryWasExecuted(ExpectedNumberOfRetries);
		}

		[Test]
		public void ShouldNotRetryIndefinitelyOnNotAuthorizedExceptionForTask()
		{
			// arrange
			const int ExpectedNumberOfRetries = 1;
			const int ExpectedNumberOfCalls = ExpectedNumberOfRetries + 1;

			int numberOfCalls = 0;
			Task Func(Context context, CancellationToken cancellationToken)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act @ assert
			Assert.That(() => this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(ExpectedNumberOfCalls));
			this.ThenOnRetryWasExecuted(ExpectedNumberOfRetries);
		}

		[Test]
		public void ShouldUseCredentialsProviderAndUpdateCredentialsOnNotAuthorizedException()
		{
			// arrange
			const int ExpectedNumberOfCalls = MaxRetryCount + 1;
			var updatedCredentials = new NetworkCredential("username", "password");
			RelativityWebApiCredentialsProvider.Instance().SetProvider(new TokenProvider(updatedCredentials));

			int numberOfCalls = 0;
			Task<string> Func(Context context, CancellationToken cancellationToken)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act & assert
			Assert.That(() => this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(ExpectedNumberOfCalls), "It should retry defined number of times, when credentials provider is registered.");
			this.serviceProxyFactoryMock.Verify(x => x.UpdateCredentials(updatedCredentials), Times.Exactly(MaxRetryCount));
			this.ThenOnRetryWasExecuted(MaxRetryCount);
		}

		[Test]
		public void ShouldNotRetryTwiceOnNotAuthorizedExceptionWhenCredentialsProviderNotRegistered()
		{
			// arrange
			const int ExpectedNumberOfRetries = 1;
			int numberOfCalls = 0;

			Task<string> Func(Context context, CancellationToken cancellationToken)
			{
				numberOfCalls++;
				throw new NotAuthorizedException();
			}

			// act & assert
			Assert.That(() => this.policy.ExecuteAsync(Func, this.context, this.cancellationTokenSource.Token), Throws.Exception.InstanceOf<NotAuthorizedException>());
			Assert.That(numberOfCalls, Is.EqualTo(2), "It should do only one retry when credentials provider is not registered.");
			this.serviceProxyFactoryMock.Verify(x => x.UpdateCredentials(It.IsAny<NetworkCredential>()), Times.Never);
			this.ThenOnRetryWasExecuted(ExpectedNumberOfRetries);
		}

		private void ThenOnRetryWasExecuted(int numberOfExpectedRetries)
		{
			if (this.useCustomOnRetry)
			{
				if (numberOfExpectedRetries > 0)
				{
					Assert.That(this.context.ContainsKey(RetryCountKey), Is.True);
					Assert.That(this.context[RetryCountKey], Is.EqualTo(numberOfExpectedRetries));
				}
				else
				{
					Assert.That(this.context.ContainsKey(RetryCountKey), Is.False);
				}
			}
			else
			{
				this.logger.Verify(x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Exactly(numberOfExpectedRetries));
			}
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