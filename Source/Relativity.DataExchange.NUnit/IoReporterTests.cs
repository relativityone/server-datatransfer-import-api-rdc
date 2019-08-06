// -----------------------------------------------------------------------------------------------------
// <copyright file="IoReporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="IoReporter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	/// <summary>
	/// Represents <see cref="IoReporter"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class IoReporterTests
	{
		private const string TestFileName = "TestFileName";
		private const string ExpectedDefaultExceptionMessage = "Expected exception message";
		private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		private IIoReporter ioReporterInstance;
		private Mock<IFileSystem> mockFileSystem;
		private Mock<IWaitAndRetryPolicy> mockWaitAndRetryPolicy;
		private Mock<IAppSettings> mockAppSettings;
		private IWaitAndRetryPolicy waitAndRetry;
		private Mock<ILog> mockLogger;
		private IoReporterContext context;
		private long actualFileLength;
		private Func<int, TimeSpan> actualRetryDurationFunc = null;
		private Exception expectedException;
		private bool actualFileExists;
		private Exception actualLoggedWarningException;
		private string actualLoggedWarningMessage;
		private Exception actualLoggedErrorException;
		private string actualLoggedErrorMessage;
		private string actualLoggedInformationMessage;
		private IAppSettings cachedAppSettings;

		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Performance",
			"CA1811:AvoidUncalledPrivateCode",
			Justification = "This is used via NUnit's TestCaseSource feature.")]
		private static IEnumerable RetryExceptionTestCases
		{
			get
			{
				const int MaxExpectedRetryCount = 5;
				const int NoExpectedRetryCount = 0;

				// Test Case: exception type should never retry.
				yield return new TestCaseData(RetryOptions.All, false, new InvalidOperationException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new InvalidOperationException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.None, false, new InvalidOperationException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);

				// Test Case: disk full scenario 1 follow the options.
				yield return new TestCaseData(RetryOptions.All, false, new System.IO.IOException(ExpectedDefaultExceptionMessage, DataExchange.ExceptionHelper.DiskFullHResultHResult), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new System.IO.IOException(ExpectedDefaultExceptionMessage, DataExchange.ExceptionHelper.DiskFullHResultHResult), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.DiskFull, false, new System.IO.IOException(ExpectedDefaultExceptionMessage, DataExchange.ExceptionHelper.DiskFullHResultHResult), MaxExpectedRetryCount);

				// Test Case: disk full scenario 2 follow the options.
				yield return new TestCaseData(RetryOptions.All, false, new System.IO.IOException(ExpectedDefaultExceptionMessage, DataExchange.ExceptionHelper.HandleDiskFullHResult), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.DiskFull, false, new System.IO.IOException(ExpectedDefaultExceptionMessage, DataExchange.ExceptionHelper.HandleDiskFullHResult), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new System.IO.IOException(ExpectedDefaultExceptionMessage, DataExchange.ExceptionHelper.HandleDiskFullHResult), NoExpectedRetryCount);

				// Test Case: file does not exist follow the options.
				yield return new TestCaseData(RetryOptions.All, false, new System.IO.FileNotFoundException(ExpectedDefaultExceptionMessage), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.FileNotFound, false, new System.IO.FileNotFoundException(ExpectedDefaultExceptionMessage), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new System.IO.FileNotFoundException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);

				// Test Case: path too long should never retry.
				yield return new TestCaseData(RetryOptions.All, false, new System.IO.PathTooLongException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new System.IO.PathTooLongException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.None, false, new System.IO.PathTooLongException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);

				// Test Case: illegal characters in the path should never retry.
				yield return new TestCaseData(RetryOptions.None, false, new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.None, true, new FileInfoInvalidPathException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.All, false, new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.All, true, new FileInfoInvalidPathException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, true, new FileInfoInvalidPathException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.None, false, new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.None, true, new FileInfoInvalidPathException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.All, false, new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.All, true, new FileInfoInvalidPathException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, true, new FileInfoInvalidPathException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage), NoExpectedRetryCount);

				// Test Case: permission errors follow the options.
				yield return new TestCaseData(RetryOptions.All, false, new UnauthorizedAccessException(ExpectedDefaultExceptionMessage), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Permissions, false, new UnauthorizedAccessException(ExpectedDefaultExceptionMessage), MaxExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.Io, false, new UnauthorizedAccessException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);
				yield return new TestCaseData(RetryOptions.None, false, new UnauthorizedAccessException(ExpectedDefaultExceptionMessage), NoExpectedRetryCount);
			}
		}

		[SetUp]
		public void Setup()
		{
			AppSettings.Instance.EnforceMinRetryCount = false;
			AppSettings.Instance.EnforceMinWaitTime = false;
			this.cachedAppSettings = new AppDotNetSettings();
			this.cachedAppSettings.EnforceMinRetryCount = false;
			this.cachedAppSettings.EnforceMinWaitTime = false;
			this.actualFileExists = false;
			this.actualFileLength = 0;
			this.actualLoggedErrorException = null;
			this.actualLoggedErrorMessage = null;
			this.actualLoggedInformationMessage = null;
			this.actualLoggedWarningException = null;
			this.actualLoggedWarningMessage = null;
			this.actualRetryDurationFunc = null;
			this.mockAppSettings = new Mock<IAppSettings>();
			this.mockAppSettings.SetupGet(x => x.DisableThrowOnIllegalCharacters).Returns(false);
			this.mockAppSettings.Setup(x => x.DeepCopy()).Callback(
				() =>
					{
						// Must sync the key I/O settings since deep copies are used.
						this.cachedAppSettings.IoErrorNumberOfRetries =
							this.mockAppSettings.Object.IoErrorNumberOfRetries;
						this.cachedAppSettings.IoErrorWaitTimeInSeconds =
							this.mockAppSettings.Object.IoErrorWaitTimeInSeconds;
						this.cachedAppSettings.DisableThrowOnIllegalCharacters =
							this.mockAppSettings.Object.DisableThrowOnIllegalCharacters;
					}).Returns(this.cachedAppSettings);
			this.mockFileSystem = new Mock<IFileSystem>();
			this.mockWaitAndRetryPolicy = new Mock<IWaitAndRetryPolicy>();
			this.waitAndRetry = null;
			this.mockLogger = new Mock<ILog>();
			this.mockLogger.Setup(
					log => log.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
				.Callback<Exception, string, object[]>(
					(ex, logWarningMessage, param) =>
					{
						this.actualLoggedWarningException = ex;
						this.actualLoggedWarningMessage = logWarningMessage;
					});
			this.mockLogger.Setup(
					log => log.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()))
				.Callback<Exception, string, object[]>((ex, logWarningMessage, param) =>
				{
					this.actualLoggedErrorException = ex;
					this.actualLoggedErrorMessage = logWarningMessage;
				});
			this.mockLogger.Setup(
					log => log.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()))
				.Callback<string, object[]>((logInformationMessage, param) =>
				{
					this.actualLoggedInformationMessage = logInformationMessage;
				});
			this.context = new IoReporterContext(
							   this.mockFileSystem.Object,
							   this.mockAppSettings.Object,
							   this.mockWaitAndRetryPolicy.Object)
			{ RetryOptions = RetryOptions.Io };
		}

		[Test]
		public void ItShouldThrowWhenTheConstructorArgAreInvalid()
		{
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						this.ioReporterInstance = new IoReporter(null, this.mockLogger.Object, CancellationToken.None);
					});
		}

		[Test]
		public void ItShouldThrowWhenTheCopyArgsAreInvalid()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				this.GivenTheIoReportInstanceIsConstructed();
				this.ioReporterInstance.CopyFile(null, "1", false, 0);
			});

			Assert.Throws<ArgumentNullException>(() =>
			{
				this.GivenTheIoReportInstanceIsConstructed();
				this.ioReporterInstance.CopyFile("1", null, false, 0);
			});

			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				this.GivenTheIoReportInstanceIsConstructed();
				this.ioReporterInstance.CopyFile("1", "2", false, -1);
			});
		}

		[Test]
		[TestCase("")]
		[TestCase(null)]
		public void ItShouldReturnFalseWhenFileNameIsNullOrEmpty(string fileName)
		{
			this.GivenTheIoReportInstanceIsConstructed();
			this.WhenCallingTheGetFileExistsReporterMethod(fileName);
			this.ThenTheActualFileExistsShouldEqual(false);
		}

		[Test]
		public void ItShouldThrowWhenTheGetFileExistsArgsAreInvalid()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
				{
					this.GivenTheIoReportInstanceIsConstructed();
					this.ioReporterInstance.GetFileExists("1", -1);
				});
		}

		[Test]
		public void ItShouldThrowWhenTheGetFileLengthArgsAreInvalid()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				this.GivenTheIoReportInstanceIsConstructed();
				this.ioReporterInstance.GetFileLength("1", -1);
			});
		}

		[Test]
		[TestCase("")]
		[TestCase(null)]
		public void ItShouldReturnZeroWhenFileNameIsNullOrEmpty(string fileName)
		{
			this.GivenTheIoReportInstanceIsConstructed();
			this.WhenCallingTheFileLengthReporterMethod(fileName);
			this.ThenTheActualFileLengthShouldEqual(0);
		}

		[TestCase(1)]
		[TestCase(10)]
		[TestCase(1299)]
		public void ItShouldGetTheFileLength(int expectedLength)
		{
			this.GivenTheRealWaitAndRetryPolicy(1);
			this.GivenTheMockFileSystemCreateFileInfoReturns(expectedLength, true);
			this.GivenTheDisableNativeLocationValidationConfigSetting(false);
			this.GivenTheRetryOptions(RetryOptions.Io);
			this.GivenTheIoReportInstanceIsConstructed();
			this.WhenCallingTheFileLengthReporterMethod(TestFileName);
			this.ThenTheActualFileLengthShouldEqual(expectedLength);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldGetTheFileExists(bool expectedFileExists)
		{
			this.GivenTheRealWaitAndRetryPolicy(1);
			this.GivenTheMockFileSystemCreateFileInfoReturns(1, expectedFileExists);
			this.GivenTheDisableNativeLocationValidationConfigSetting(false);
			this.GivenTheRetryOptions(RetryOptions.Io);
			this.GivenTheIoReportInstanceIsConstructed();
			this.WhenCallingTheGetFileExistsReporterMethod(TestFileName);
			this.ThenTheActualFileExistsShouldEqual(expectedFileExists);
		}

		[TestCase(1, 1)]
		[TestCase(2, 1)]
		[TestCase(2, 2)]
		[TestCase(3, 10)]
		[TestCase(2, -1)]
		[TestCase(2, 0)]
		public void ItShouldCalculateTheProperRetryDuration(int retryAttempt, int waitTimeBetweenRetryAttempts)
		{
			this.GivenTheMockWaitAndRetryReturns(waitTimeBetweenRetryAttempts);
			this.GivenTheMockWaitAndRetryPolicyCallback();
			this.GivenTheMockFileSystemCreateFileInfoReturns(1000, true);
			this.GivenTheDisableNativeLocationValidationConfigSetting(false);
			this.GivenTheRetryOptions(RetryOptions.Io);
			this.GivenTheIoReportInstanceIsConstructed();
			this.WhenCallingTheFileLengthReporterMethod(TestFileName);
			int expectedWaitTimeBetweenRetryAttempts = waitTimeBetweenRetryAttempts;
			this.ThenTheActualRetryDurationShouldCalculated(retryAttempt, expectedWaitTimeBetweenRetryAttempts);
		}

		[Test]
		public void ItShouldReturnWhenCanceled()
		{
			this.cancellationTokenSource.Cancel();
			this.GivenTheRealWaitAndRetryPolicy(1);
			this.GivenTheMockFileSystemCreateFileInfoReturns(1, true);
			this.GivenTheDisableNativeLocationValidationConfigSetting(false);
			this.GivenTheRetryOptions(RetryOptions.None);
			this.GivenTheIoReportInstanceIsConstructed();
			this.WhenCallingTheFileLengthReporterMethod(TestFileName);
			this.ThenTheActualFileLengthShouldEqual(default(int));
			this.ThenTheLoggerInformationShouldBeInvoked(1);
		}

		[Test]
		[TestCaseSource(nameof(RetryExceptionTestCases))]
		public void ItShouldExerciseRetryExceptionTestCases(RetryOptions testOptions, bool testDisableNativeLocationValidation, Exception testException, int testExpectedRetryCount)
		{
			if (testException == null)
			{
				throw new ArgumentNullException(nameof(testException));
			}

			const int MaxRetryCount = 5;
			this.GivenTheRetryOptions(testOptions);
			this.GivenTheRealWaitAndRetryPolicy(MaxRetryCount);
			this.GivenTheMockFileSystemCreateFileInfoThrows(testException);
			this.GivenTheMockFileSystemCopyThrows(testException);
			this.GivenTheDisableNativeLocationValidationConfigSetting(testDisableNativeLocationValidation);
			this.GivenTheIoReportInstanceIsConstructed();

			// Verify copying a file.
			Assert.Throws(testException.GetType(), this.WhenCallingTheFileCopyReporterMethod);
			bool localExpectedException = !testDisableNativeLocationValidation;
			this.ThenTheLoggerErrorShouldBeInvoked(0, localExpectedException);
			this.ThenTheLoggerWarningShouldBeInvoked(testExpectedRetryCount, localExpectedException);
			this.ResetMockLogger();

			// Verify retrieving the file length.
			Assert.Throws(testException.GetType(), () => this.WhenCallingTheFileLengthReporterMethod(TestFileName));
			this.ThenTheLoggerErrorShouldBeInvoked(0, localExpectedException);
			this.ThenTheLoggerWarningShouldBeInvoked(testExpectedRetryCount, localExpectedException);
			this.ResetMockLogger();

			// Verify retrieving the file exists flag.
			Assert.Throws(testException.GetType(), () => this.WhenCallingTheGetFileExistsReporterMethod(TestFileName));
			this.ThenTheLoggerErrorShouldBeInvoked(0, localExpectedException);
			this.ThenTheLoggerWarningShouldBeInvoked(testExpectedRetryCount, localExpectedException);
			this.ResetMockLogger();
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldHandleIllegalCharactersInPathException(bool testValue)
		{
			Exception exception = new ArgumentException(DataExchange.ExceptionHelper.IllegalCharactersInPathMessage);
			this.GivenTheDisableNativeLocationValidationConfigSetting(testValue);
			this.GivenTheRetryOptions(RetryOptions.Io);
			this.GivenTheExpectedException(exception);
			this.GivenTheMockFileSystemCreateFileInfoThrows(exception);
			this.GivenTheMockWaitAndRetryCallback();
			this.GivenTheIoReportInstanceIsConstructed();
			if (testValue)
			{
				Assert.Throws<FileInfoInvalidPathException>(() => this.WhenCallingTheFileLengthReporterMethod(TestFileName));
			}
			else
			{
				Assert.Throws<ArgumentException>(() => this.WhenCallingTheFileLengthReporterMethod(TestFileName));
			}

			const bool ExpectedException = false;
			this.ThenTheLoggerErrorShouldBeInvoked(testValue ? 1 : 0, ExpectedException);
			this.ThenTheLoggerWarningShouldBeInvoked(1, ExpectedException);
		}

		private void GivenTheMockWaitAndRetryReturns(int waitTimeBetweenRetryAttempts)
		{
			this.mockAppSettings.SetupGet(x => x.IoErrorWaitTimeInSeconds).Returns(waitTimeBetweenRetryAttempts);
		}

		/// <summary>
		/// Givens the real wait and retry policy.
		/// </summary>
		/// <param name="maxRetryAttempts">
		/// The maximum retry attempts.
		/// </param>
		private void GivenTheRealWaitAndRetryPolicy(int maxRetryAttempts)
		{
			this.mockAppSettings.SetupGet(x => x.IoErrorNumberOfRetries).Returns(maxRetryAttempts);
			this.mockAppSettings.SetupGet(x => x.IoErrorWaitTimeInSeconds).Returns(0);
			this.waitAndRetry = new WaitAndRetryPolicy(this.mockAppSettings.Object);
		}

		private void GivenTheMockWaitAndRetryPolicyCallback()
		{
			this.mockWaitAndRetryPolicy
				.Setup(
					obj => obj.WaitAndRetry(
						It.IsAny<Func<Exception, bool>>(),
						It.IsAny<Func<int, TimeSpan>>(),
						It.IsAny<Action<Exception, TimeSpan>>(),
						It.IsAny<Func<CancellationToken, long>>(),
						It.IsAny<CancellationToken>()))
				.Callback<
					Func<Exception, bool>,
					Func<int, TimeSpan>,
					Action<Exception, TimeSpan>,
					Func<CancellationToken, long>,
					CancellationToken>(
					(exceptionPredicate, retryDuration, retryAction, execFunc, token) =>
					{
						this.actualRetryDurationFunc = retryDuration;
						retryAction(this.expectedException, TimeSpan.Zero);
						execFunc(token);
					});
		}

		private void GivenTheMockFileSystemCreateFileInfoReturns(int expectedLength, bool expectedFileExists)
		{
			var mockFileInfo = new Mock<IFileInfo>();
			mockFileInfo.Setup(x => x.Length).Returns(expectedLength);
			mockFileInfo.Setup(x => x.Exists).Returns(expectedFileExists);
			this.mockFileSystem.Setup(x => x.CreateFileInfo(TestFileName)).Returns(mockFileInfo.Object);
		}

		private void GivenTheMockFileSystemCreateFileInfoThrows(Exception exception)
		{
			this.mockFileSystem.Setup(x => x.CreateFileInfo(It.IsAny<string>())).Throws(exception);
		}

		private void GivenTheMockFileSystemCopyThrows(Exception exception)
		{
			var mockFile = new Mock<IFile>();
			mockFile.Setup(x => x.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(exception);
			this.mockFileSystem.Setup(x => x.File).Returns(mockFile.Object);
		}

		private void GivenTheMockWaitAndRetryCallback()
		{
			this.mockWaitAndRetryPolicy
				.Setup(
					obj => obj.WaitAndRetry(
						It.IsAny<Func<Exception, bool>>(),
						It.IsAny<Func<int, TimeSpan>>(),
						It.IsAny<Action<Exception, TimeSpan>>(),
						It.IsAny<Func<CancellationToken, long>>(),
						It.IsAny<CancellationToken>()))
				.Callback<Func<Exception, bool>, Func<int, TimeSpan>, Action<Exception, TimeSpan>,
					Func<CancellationToken, long>, CancellationToken>(
					(exceptionPredicate, retryDuration, retryAction, execFunc, token) =>
					{
						this.actualRetryDurationFunc = retryDuration;
						retryAction(null, TimeSpan.Zero);
						execFunc(token);
					});
		}

		private void GivenTheExpectedException(Exception exception)
		{
			this.expectedException = exception;
		}

		private void GivenTheDisableNativeLocationValidationConfigSetting(bool value)
		{
			this.mockAppSettings.SetupGet(x => x.DisableThrowOnIllegalCharacters).Returns(value);
		}

		private void GivenTheRetryOptions(RetryOptions value)
		{
			this.context.RetryOptions = value;
		}

		private void GivenTheIoReportInstanceIsConstructed()
		{
			this.context.WaitAndRetryPolicy = this.waitAndRetry ?? this.mockWaitAndRetryPolicy.Object;
			this.ioReporterInstance = new IoReporter(
				this.context,
				this.mockLogger.Object,
				this.cancellationTokenSource.Token);
		}

		private void WhenCallingTheFileLengthReporterMethod(string fileName)
		{
			this.actualFileLength = this.ioReporterInstance.GetFileLength(fileName, 0);
		}

		private void WhenCallingTheGetFileExistsReporterMethod(string fileName)
		{
			this.actualFileExists = this.ioReporterInstance.GetFileExists(fileName, 0);
		}

		private void WhenCallingTheFileCopyReporterMethod()
		{
			this.ioReporterInstance.CopyFile("source", "destination", true, 0);
		}

		private void ThenTheActualRetryDurationShouldCalculated(int retryAttempt, int waitTimeBetweenRetryAttempts)
		{
			TimeSpan actualRetryDuraction = this.actualRetryDurationFunc(retryAttempt);
			Assert.That(
				actualRetryDuraction,
				retryAttempt == 1 ? Is.EqualTo(TimeSpan.FromSeconds(0)) : Is.EqualTo(TimeSpan.FromSeconds(waitTimeBetweenRetryAttempts)));
		}

		private void ThenTheActualFileLengthShouldEqual(int expectedLength)
		{
			Assert.That(this.actualFileLength, Is.EqualTo(expectedLength));
		}

		private void ThenTheActualFileExistsShouldEqual(bool expectedFileExists)
		{
			Assert.That(this.actualFileExists, Is.EqualTo(expectedFileExists));
		}

		private void ThenTheLoggerWarningShouldBeInvoked(int expectedCount, bool expectedLogException)
		{
			this.mockLogger.Verify(logger => logger.LogWarning(It.IsAny<Exception>(), It.IsAny<string>()), Times.Exactly(expectedCount));
			if (expectedCount == 0)
			{
				if (expectedLogException)
				{
					Assert.That(this.actualLoggedWarningException, Is.Null);
				}

				Assert.That(this.actualLoggedWarningMessage, Is.Null.Or.Empty);
			}
			else
			{
				if (expectedLogException)
				{
					Assert.That(this.actualLoggedWarningException, Is.Not.Null);
				}

				Assert.That(this.actualLoggedWarningMessage, Is.Not.Null.Or.Empty);
			}
		}

		private void ThenTheLoggerInformationShouldBeInvoked(int expectedCount)
		{
			this.mockLogger.Verify(logger => logger.LogInformation(It.IsAny<string>()), Times.Exactly(expectedCount));
			Assert.That(
				this.actualLoggedInformationMessage,
				expectedCount == 0 ? Is.Null.Or.Empty : Is.Not.Null.Or.Empty);
		}

		private void ThenTheLoggerErrorShouldBeInvoked(int expectedCount, bool expectedLogException)
		{
			this.mockLogger.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Exactly(expectedCount));
			if (expectedCount == 0)
			{
				if (expectedLogException)
				{
					Assert.That(this.actualLoggedErrorException, Is.Null);
				}

				Assert.That(this.actualLoggedErrorMessage, Is.Null.Or.Empty);
			}
			else
			{
				if (expectedLogException)
				{
					Assert.That(this.actualLoggedErrorException, Is.Not.Null);
				}

				Assert.That(this.actualLoggedErrorMessage, Is.Not.Null.Or.Empty);
			}
		}

		private void ResetMockLogger()
		{
			this.mockLogger.ResetCalls();
			this.actualLoggedErrorException = null;
			this.actualLoggedErrorMessage = null;
			this.actualLoggedWarningException = null;
			this.actualLoggedWarningMessage = null;
		}
	}
}