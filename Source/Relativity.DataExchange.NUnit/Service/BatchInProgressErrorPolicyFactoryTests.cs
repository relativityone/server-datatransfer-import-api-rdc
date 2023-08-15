// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchInProgressErrorPolicyFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="BatchInProgressErrorPolicyFactoryTests"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit.Service
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Polly;

	using Relativity.DataExchange.Service;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;

	/// <summary>
	/// Represents <see cref="BatchInProgressErrorPolicyFactoryTests"/> tests.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	[TestFixture(true)]
	[TestFixture(false)]
	public class BatchInProgressErrorPolicyFactoryTests
	{
		private const string RetryCountKey = "TestRetryCount";
		private const int MaxRetryCount = 3;
		private readonly bool useCustomOnRetry;
		private Mock<IAppSettings> settings;
		private Mock<ILog> logger;
		private IAsyncPolicy policy;
		private CancellationTokenSource cancellationTokenSource;
		private Context context;

		public BatchInProgressErrorPolicyFactoryTests(bool useCustomOnRetry)
		{
			this.useCustomOnRetry = useCustomOnRetry;
		}

		private static IEnumerable FatalExceptions
		{
			get
			{
				return ExceptionHelper.FatalExceptionCandidates.Where(x => x != typeof(ThreadAbortException))
					.Concat(ExceptionHelper.FatalKeplerExceptionCandidates).Select(Activator.CreateInstance)
                    .Concat(new[] { new ArgumentException("Bearer token should not be null or empty.") })
                    .Concat(new[] { new OperationCanceledException("test") });
			}
		}

		private static IEnumerable NonFatalHttpExceptions
		{
			get
			{
				yield return new ConflictException("Batch In Progress Exception");
				yield return new TaskCanceledException("Test");
			}
		}

		[SetUp]
		public void Setup()
		{
			this.cancellationTokenSource = new CancellationTokenSource();
			this.context = new Context("test");
			this.settings = new Mock<IAppSettings>();
			this.logger = new Mock<ILog>();

			this.settings.SetupGet(x => x.BatchInProgressWaitTimeInSeconds).Returns(0);
			this.settings.SetupGet(x => x.BatchInProgressNumberOfRetries).Returns(MaxRetryCount);
			this.policy = this.useCustomOnRetry ?
				              new BatchInProgressErrorPolicyFactory(this.settings.Object, this.logger.Object, (exception, duration, retryCount, ctx) => { ctx[RetryCountKey] = retryCount; }).CreateRetryPolicy() :
				              new BatchInProgressErrorPolicyFactory(this.settings.Object, this.logger.Object).CreateRetryPolicy();
		}

		[TearDown]
		public void TearDown()
		{
			this.cancellationTokenSource?.Dispose();
		}

		[Test]
		public async Task ShouldExecuteWithoutRetryAsync()
		{
			// ACT
			int result = await this.policy.ExecuteAsync(
							 (ctx, ct) => Task.FromResult(1),
							 this.context,
							 this.cancellationTokenSource.Token).ConfigureAwait(false);

			// ASSERT
			Assert.That(result, Is.EqualTo(1));
			this.ThenOnRetryWasExecuted(0);
		}

		[TestCaseSource(nameof(NonFatalHttpExceptions))]
		public void ShouldExecuteWithRetryOnNonFatalException(Exception exception)
		{
			// ARRANGE
			Assert.That(exception, Is.Not.Null);

			// ACT
			Assert.ThrowsAsync(
				exception.GetType(),
				async () =>
				{
					await this.policy.ExecuteAsync(
						(ctx, ct) => throw exception,
						this.context,
						this.cancellationTokenSource.Token).ConfigureAwait(false);
				});

			// ASSERT
			this.ThenOnRetryWasExecuted(MaxRetryCount);
		}

		[TestCaseSource(nameof(NonFatalHttpExceptions))]
		public async Task ShouldExecuteWithFailureThenSuccessOnNonFatalExceptionAsync(Exception exception)
		{
			// ARRANGE
			Assert.That(exception, Is.Not.Null);
			bool shouldThrowException = true;

			// ACT
			await this.policy.ExecuteAsync(
				(ctx, ct) =>
				{
					if (shouldThrowException)
					{
						shouldThrowException = false;
						throw exception;
					}

					return Task.CompletedTask;
				},
				this.context,
				this.cancellationTokenSource.Token).ConfigureAwait(false);

			// ASSERT
			this.ThenOnRetryWasExecuted(1);
		}

		[TestCaseSource(nameof(FatalExceptions))]
		public void ShouldExecuteWithoutRetryOnFatalException(Exception exception)
		{
			Assert.That(exception, Is.Not.Null);

			// ACT
			Assert.ThrowsAsync(
				exception.GetType(),
				async () =>
				{
					await this.policy.ExecuteAsync(
						(ctx, ct) => throw exception,
						this.context,
						this.cancellationTokenSource.Token).ConfigureAwait(false);
				});

			// ASSERT
			this.ThenOnRetryWasExecuted(0);
		}

		[Test]
		public void ShouldExecuteWithoutRetryOnCancellationRequest()
		{
			// ARRANGE
			this.cancellationTokenSource.Cancel();

			// ACT
			Assert.ThrowsAsync<OperationCanceledException>(
				async () => await this.policy.ExecuteAsync(
								(ctx, ct) => Task.FromResult(1),
								this.context,
								this.cancellationTokenSource.Token).ConfigureAwait(false));

			// ASSERT
			this.ThenOnRetryWasExecuted(0);
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
				this.logger.Verify(x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Exactly(numberOfExpectedRetries));
			}
		}
	}
}