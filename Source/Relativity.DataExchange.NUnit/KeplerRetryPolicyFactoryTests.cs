// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeplerRetryPolicyFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="KeplerRetryPolicyFactory"/> tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Polly;

	using Relativity.DataExchange.Service;

	/// <summary>
	/// Represents <see cref="KeplerRetryPolicyFactory"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class KeplerRetryPolicyFactoryTests
	{
		private const string RetryCountKey = "TestRetryCount";
		private const int MaxRetryCount = 3;
		private Mock<IAppSettings> settings;
		private IAsyncPolicy policy;
		private CancellationTokenSource cancellationTokenSource;
		private Context context;

		private static IEnumerable FatalExceptions
		{
			get
			{
				return ExceptionHelper.FatalExceptionCandidates.Where(x => x != typeof(ThreadAbortException))
					.Concat(ExceptionHelper.FatalKeplerExceptionCandidates).Select(Activator.CreateInstance);
			}
		}

		private static IEnumerable NonFatalHttpExceptions
		{
			get
			{
				yield return new Relativity.Services.Exceptions.ServiceException("Test");
				yield return new System.Net.Http.HttpRequestException("Test");
				yield return new System.Net.HttpListenerException((int)HttpStatusCode.BadGateway, "Test");
				yield return new WebException("Test", WebExceptionStatus.ConnectFailure);
			}
		}

		[SetUp]
		public void Setup()
		{
			this.cancellationTokenSource = new CancellationTokenSource();
			this.context = new Context("test");
			this.settings = new Mock<IAppSettings>();
			this.settings.SetupGet(x => x.HttpErrorWaitTimeInSeconds).Returns(0);
			this.settings.SetupGet(x => x.HttpErrorNumberOfRetries).Returns(MaxRetryCount);
			this.policy = new KeplerRetryPolicyFactory(this.settings.Object).CreateAsyncPolicy(
				(exception, duration, retryCount, ctx) => { ctx[RetryCountKey] = retryCount; });
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
			Assert.That(this.context.ContainsKey(RetryCountKey), Is.False);
		}

		[TestCaseSource(nameof(NonFatalHttpExceptions))]
		public void ShouldExecuteWithRetryOnNonFatalHttpException(Exception exception)
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
			Assert.That(this.context.ContainsKey(RetryCountKey), Is.True);
			Assert.That(this.context[RetryCountKey], Is.EqualTo(MaxRetryCount));
		}

		[TestCaseSource(nameof(NonFatalHttpExceptions))]
		public async Task ShouldExecuteWithFailureThenSuccessOnNonFatalHttpExceptionAsync(Exception exception)
		{
			// ARRANGE
			Assert.That(exception, Is.Not.Null);

			// ACT
			await this.policy.ExecuteAsync(
				(ctx, ct) =>
					{
						if (!ctx.ContainsKey(RetryCountKey))
						{
							throw exception;
						}

						return Task.CompletedTask;
					},
				this.context,
				this.cancellationTokenSource.Token).ConfigureAwait(false);

			// ASSERT
			Assert.That(this.context.ContainsKey(RetryCountKey), Is.True);
			Assert.That(this.context[RetryCountKey], Is.EqualTo(1));
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
			Assert.That(this.context.ContainsKey(RetryCountKey), Is.False);
		}

		[Test]
		public void ShouldExecuteWithoutRetryOnCancellationRequest()
		{
			// ARRANGE
			this.cancellationTokenSource.Cancel();

			// ACT
			Assert.ThrowsAsync<TaskCanceledException>(
				async () => await this.policy.ExecuteAsync(
					            (ctx, ct) => Task.FromResult(1),
					            this.context,
					            this.cancellationTokenSource.Token).ConfigureAwait(false));

			// ASSERT
			Assert.That(this.context.ContainsKey(RetryCountKey), Is.False);
		}
	}
}