// -----------------------------------------------------------------------------------------------------
// <copyright file="WaitAndRetryPolicyTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="WaitAndRetryPolicy"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Threading;

	using global::NUnit.Framework;

	using Relativity.DataExchange;

	[TestFixture]
	public class WaitAndRetryPolicyTests
	{
		private int waitTimeMillisecondsBetweenRetryAttempts;
		private int actualRetryCallCount;
		private int actualExecFuncCallCount;
		private int expectedRetryCallCount;
		private int expectedExecFuncCallCount;
		private Func<int, TimeSpan> retryDuration;
		private Action<Exception, TimeSpan> retryAction;
		private Action<CancellationToken> execFunc;

		[SetUp]
		public void Setup()
		{
			// DO NOT INCREASE this VALUE BECAUSE IT WILL INCREASE THE TESTS EXECUTION TIME
			this.waitTimeMillisecondsBetweenRetryAttempts = 1;
			this.actualRetryCallCount = 0;
			this.actualExecFuncCallCount = 0;
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(2)]
		[TestCase(12)]
		public void ItShouldWaitAndRetryWithMethodParamsSuccess(int maxRetryCount)
		{
			this.GivenTheExpectedRetryCallCount(0);
			this.GivenTheExpectedExecFuncCallCount(1);

			this.GivenTheRetryDuration();
			this.GivenTheRetryAction();
			this.GivenTheExecFunc((token) => { this.actualExecFuncCallCount++; });

			// Act
			this.WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(maxRetryCount);

			// Assert
			this.ThenTheActualRetryCallCountShouldEqual();
			this.ThenTheActualExecFuncCallCountShouldEqual();
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(2)]
		[TestCase(12)]
		public void ItShouldWaitAndRetryWithConstructorParamsSuccess(int maxRetryCount)
		{
			this.GivenTheExpectedRetryCallCount(0);
			this.GivenTheExpectedExecFuncCallCount(1);

			this.GivenTheRetryDuration();
			this.GivenTheRetryAction();
			this.GivenTheExecFunc((token) => { this.actualExecFuncCallCount++; });

			this.WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(maxRetryCount);

			this.ThenTheActualRetryCallCountShouldEqual();
			this.ThenTheActualExecFuncCallCountShouldEqual();
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(10)]
		public void ItShouldWaitAndRetryWithMethodParamsAlwaysFails(int maxRetryCount)
		{
			this.GivenTheExpectedRetryCallCount(maxRetryCount);
			this.GivenTheExpectedExecFuncCallCount(checked(maxRetryCount + 1));

			this.GivenTheRetryDuration();
			this.GivenTheRetryAction();
			this.GivenTheExecFunc((token) =>
			{
				this.actualExecFuncCallCount++;
				throw new InvalidOperationException();
			});

			this.WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParamsThenThwowsException(maxRetryCount);

			this.ThenTheActualRetryCallCountShouldEqual();
			this.ThenTheActualExecFuncCallCountShouldEqual();
		}

		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(10)]
		public void ItShouldWaitAndRetryWithConstructorParamsAlwaysFails(int maxRetryCount)
		{
			this.GivenTheExpectedRetryCallCount(maxRetryCount);
			this.GivenTheExpectedExecFuncCallCount(checked(maxRetryCount + 1));

			this.GivenTheRetryDuration();
			this.GivenTheRetryAction();
			this.GivenTheExecFunc((token) =>
			{
				this.actualExecFuncCallCount++;
				throw new InvalidOperationException();
			});

			this.WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParamsThenThrowsException(maxRetryCount);

			this.ThenTheActualRetryCallCountShouldEqual();
			this.ThenTheActualExecFuncCallCountShouldEqual();
		}

		[TestCase(2, 1)]
		[TestCase(3, 2)]
		[TestCase(10, 5)]
		public void ItShouldWaitAndRetryWithMethodParamsNotAllFails(int maxRetryCount, int succesAfterRetryNum)
		{
			this.GivenTheExpectedRetryCallCount(succesAfterRetryNum);
			this.GivenTheExpectedExecFuncCallCount(succesAfterRetryNum + 1);

			this.GivenTheRetryDuration();
			this.GivenTheRetryAction();
			this.GivenTheExecFunc((token) =>
			{
				this.actualExecFuncCallCount++;
				if (this.actualExecFuncCallCount <= succesAfterRetryNum)
				{
					throw new InvalidOperationException();
				}
			});

			this.WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(maxRetryCount);

			this.ThenTheActualRetryCallCountShouldEqual();
			this.ThenTheActualExecFuncCallCountShouldEqual();
		}

		[TestCase(2, 1)]
		[TestCase(3, 2)]
		[TestCase(10, 5)]
		public void ItShouldWaitAndRetryWithConstructorParamsNotAllFails(int maxRetryCount, int succesAfterRetryNum)
		{
			this.GivenTheExpectedRetryCallCount(succesAfterRetryNum);
			this.GivenTheExpectedExecFuncCallCount(succesAfterRetryNum + 1);

			this.GivenTheRetryDuration();
			this.GivenTheRetryAction();
			this.GivenTheExecFunc((token) =>
			{
				this.actualExecFuncCallCount++;
				if (this.actualExecFuncCallCount <= succesAfterRetryNum)
				{
					throw new InvalidOperationException();
				}
			});

			this.WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(maxRetryCount);

			this.ThenTheActualRetryCallCountShouldEqual();
			this.ThenTheActualExecFuncCallCountShouldEqual();
		}

		private void GivenTheExpectedRetryCallCount(int count)
		{
			this.expectedRetryCallCount = count;
		}

		private void GivenTheExpectedExecFuncCallCount(int count)
		{
			this.expectedExecFuncCallCount = count;
		}

		private void GivenTheRetryDuration()
		{
			this.retryDuration = waitTime => TimeSpan.FromMilliseconds(this.waitTimeMillisecondsBetweenRetryAttempts);
		}

		private void GivenTheRetryAction()
		{
			this.retryAction = (exception, timeSpan) => { this.actualRetryCallCount++; };
		}

		private void GivenTheExecFunc(Action<CancellationToken> action)
		{
			this.execFunc = action;
		}

		private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParams(int maxRetryCount)
		{
			AppSettings.Instance.EnforceMinRetryCount = false;
			AppSettings.Instance.EnforceMinWaitTime = false;
			IAppSettings appSettings = new AppDotNetSettings();
			appSettings.EnforceMinRetryCount = false;
			appSettings.EnforceMinWaitTime = false;
			appSettings.IoErrorNumberOfRetries = maxRetryCount;
			appSettings.IoErrorWaitTimeInSeconds = this.waitTimeMillisecondsBetweenRetryAttempts;
			WaitAndRetryPolicy waitAndRetryPolicy = new WaitAndRetryPolicy(appSettings);
			waitAndRetryPolicy.WaitAndRetry<InvalidOperationException>(
				this.retryDuration,
				this.retryAction,
				this.execFunc,
				CancellationToken.None);
		}

		private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsConstructorParamsThenThrowsException(int maxRetryCount)
		{
			IAppSettings appSettings = new AppDotNetSettings();
			appSettings.EnforceMinWaitTime = false;
			appSettings.IoErrorNumberOfRetries = maxRetryCount;
			appSettings.IoErrorWaitTimeInSeconds = this.waitTimeMillisecondsBetweenRetryAttempts;
			WaitAndRetryPolicy waitAndRetryPolicy = new WaitAndRetryPolicy(appSettings);
			Assert.That(
				() => waitAndRetryPolicy.WaitAndRetry<InvalidOperationException>(
					this.retryDuration,
					this.retryAction,
					this.execFunc,
					CancellationToken.None),
				Throws.Exception.TypeOf<InvalidOperationException>());
		}

		private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParams(int maxRetryCount)
		{
			var waitAndRetryPolicy = new WaitAndRetryPolicy();
			waitAndRetryPolicy.WaitAndRetry<InvalidOperationException>(
				maxRetryCount,
				this.retryDuration,
				this.retryAction,
				this.execFunc,
				CancellationToken.None);
		}

		private void WhenExecutingTheWaitAndRetryWithRetryCountAndDurationAsMethodParamsThenThwowsException(int maxRetryCount)
		{
			var waitAndRetryPolicy = new WaitAndRetryPolicy();
			Assert.That(
				() => waitAndRetryPolicy.WaitAndRetry<InvalidOperationException>(
					maxRetryCount,
					this.retryDuration,
					this.retryAction,
					this.execFunc,
					CancellationToken.None),
				Throws.Exception.TypeOf<InvalidOperationException>());
		}

		private void ThenTheActualRetryCallCountShouldEqual()
		{
			Assert.That(this.actualRetryCallCount, Is.EqualTo(this.expectedRetryCallCount));
		}

		private void ThenTheActualExecFuncCallCountShouldEqual()
		{
			Assert.That(this.actualExecFuncCallCount, Is.EqualTo(this.expectedExecFuncCallCount));
		}
	}
}