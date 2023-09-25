// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextStreamServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="LongTextStreamService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections;
	using System.Collections.Concurrent;
	using System.IO;
	using System.Linq;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Service.Kepler;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Kepler.Exceptions;
	using Relativity.Kepler.Transport;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	/// <summary>
	/// Represents <see cref="LongTextStreamService"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class LongTextStreamServiceTests
	{
		private ConcurrentBag<LongTextStreamProgressEventArgs> publishedProgressArgs;
		private TempDirectory2 tempDirectory;
		private MemoryStream sourceLongTextStream;
		private Mock<ILog> logger;
		private IFileSystem fileSystem;
		private Mock<IAppSettings> settings;
		private Mock<IServiceProxyFactory> factory;
		private Mock<IObjectManager> objectManager;
		private Mock<IKeplerStream> keplerStream;
		private Mock<IServiceNotification> serviceNotification;
		private LongTextStreamService sut;
		private string longTextData;
		private byte[] longTextDataBytes;
		private string targetExtractedTextFile;
		private CancellationTokenSource cancellationTokenSource;
		private Progress<LongTextStreamProgressEventArgs> progress;

		private static IEnumerable InvalidRequestParameters
		{
			get
			{
				yield return new TestCaseData(null, 1, 1, Encoding.Default, "file", 1);
				yield return new TestCaseData(Encoding.Default, 0, 1, Encoding.Default, "file", 1);
				yield return new TestCaseData(Encoding.Default, 1, 0, Encoding.Default, "file", 1);
				yield return new TestCaseData(Encoding.Default, 1, 1, null, "file", 1);
				yield return new TestCaseData(Encoding.Default, 1, 1, Encoding.Default, null, 1);
				yield return new TestCaseData(Encoding.Default, 1, 1, Encoding.Default, string.Empty, 1);
				yield return new TestCaseData(Encoding.Default, 1, 1, Encoding.Default, "file", 0);
			}
		}

		private static IEnumerable InvalidFileNames
		{
			get
			{
				foreach (char ch in new[] { '<', '>', '|', '*', '?' })
				{
					yield return $"invalid-filename-{ch}.txt";
				}
			}
		}

		[SetUp]
		public void Setup()
		{
			this.serviceNotification = new Mock<IServiceNotification>();
			this.publishedProgressArgs = new ConcurrentBag<LongTextStreamProgressEventArgs>();
			this.progress = new Progress<LongTextStreamProgressEventArgs>(args => { this.publishedProgressArgs.Add(args); });
			this.tempDirectory = new TempDirectory2();
			this.tempDirectory.Create();
			this.cancellationTokenSource = new CancellationTokenSource();
			this.longTextData = RandomHelper.NextString(50, 100);
			this.longTextDataBytes = Encoding.Unicode.GetBytes(this.longTextData);
			this.sourceLongTextStream = new MemoryStream(this.longTextDataBytes);
			this.fileSystem = new FileSystemWrap();
			this.logger = new Mock<ILog>();
			this.settings = new Mock<IAppSettings>();
			this.settings.SetupGet(x => x.ExportLongTextBufferSizeBytes).Returns(4096);
			this.settings.SetupGet(x => x.ExportLongTextLargeFileProgressRateSeconds).Returns(0);
			this.settings.SetupGet(x => x.HttpErrorWaitTimeInSeconds).Returns(0);
			this.settings.SetupGet(x => x.HttpErrorNumberOfRetries).Returns(3);
			this.keplerStream = new Mock<IKeplerStream>();
			this.keplerStream.Setup(x => x.GetStreamAsync()).ReturnsAsync(this.sourceLongTextStream);
			this.objectManager = new Mock<IObjectManager>();
			this.objectManager.Setup(
					x => x.StreamLongTextAsync(It.IsAny<int>(), It.IsAny<RelativityObjectRef>(), It.IsAny<FieldRef>()))
				.ReturnsAsync(this.keplerStream.Object);
			this.factory = new Mock<IServiceProxyFactory>();
			this.factory.Setup(x => x.CreateProxyInstance<IObjectManager>()).Returns(this.objectManager.Object);
			this.sut = new LongTextStreamService(
				this.factory.Object,
				this.serviceNotification.Object,
				this.settings.Object,
				this.fileSystem,
				this.logger.Object);
			this.targetExtractedTextFile = this.fileSystem.Path.Combine(
				this.tempDirectory.Directory,
				$"long-text-output-{Guid.NewGuid()}.txt");
		}

		[TearDown]
		public void Teardown()
		{
			this.sourceLongTextStream?.Dispose();
			this.cancellationTokenSource?.Dispose();
			this.tempDirectory?.Dispose();
			this.sut?.Dispose();
		}

		[Test]
		[TestCase("Unicode")]
        [TestCase("Windows-1252")]
        [TestCase("")]
		public async Task ShouldSaveTheLongTextStreamAsync(string encoding)
		{
			// ARRANGE
			Encoding targetEncoding =
				!string.IsNullOrWhiteSpace(encoding) ? Encoding.GetEncoding(encoding) : Encoding.Default;
			LongTextStreamRequest request = this.CreateLongTextStreamRequest(targetEncoding);

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              this.progress).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheSuccessfulLongTextResult(request, result, 0);
			this.VerifyTheStreamLongTextMethodIsCalled(1);
			this.VerifyProgressEventsArePublished(result.Length);
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		[Test]
		[TestCaseSource(nameof(InvalidFileNames))]
		public async Task ShouldNotRetryInvalidFileNamesAsync(string invalidFileName)
		{
			// ARRANGE
			LongTextStreamRequest request = this.CreateLongTextStreamRequest(
				Encoding.Unicode,
				$"{this.tempDirectory.Directory}{System.IO.Path.DirectorySeparatorChar}{invalidFileName}");

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              this.progress).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheNonFatalLongTextResult(request, result, 0);
			this.VerifyTheStreamLongTextMethodIsCalled(1);
			this.VerifyNoErrorsAreLogged();
			this.VerifyTheWarningsAreLogged(1);
			this.VerifyNoNotificationMessagesArePublished();
		}

		[TestCaseSource(
			typeof(ObjectManagerExceptionTestData),
			nameof(ObjectManagerExceptionTestData.AllExpectedObjectManagerErrors))]
		public async Task ShouldNotRetryExpectedObjectManagerErrorsAsync(Exception exception)
		{
			// ARRANGE
			this.objectManager.Setup(
					x => x.StreamLongTextAsync(It.IsAny<int>(), It.IsAny<RelativityObjectRef>(), It.IsAny<FieldRef>()))
				.Throws(exception);
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              this.progress).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheNonFatalLongTextResult(request, result, 0);
			this.VerifyTheStreamLongTextMethodIsCalled(1);
			this.VerifyProgressEventsAreNotPublished();
			this.VerifyNoErrorsAreLogged();
			this.VerifyTheWarningsAreLogged(1);
			this.VerifyNoNotificationMessagesArePublished();
		}

		[Test]
		public async Task ShouldNotThrowWhenTheFileDeleteFailsAsync()
		{
			// ARRANGE
			Mock<IFile> mockFile = new Mock<IFile>();
			mockFile.Setup(x => x.Delete(It.IsAny<string>())).Throws(new FileNotFoundException());
			mockFile.Setup(x => x.Create(It.IsAny<string>())).Returns((string path) => File.Create(path));
			Mock<IFileSystem> mockFileSystem = new Mock<IFileSystem>();
			mockFileSystem.SetupGet(x => x.File).Returns(mockFile.Object);
			this.sut = new LongTextStreamService(
				this.factory.Object,
				this.serviceNotification.Object,
				this.settings.Object,
				mockFileSystem.Object,
				this.logger.Object);
			this.objectManager
				.SetupSequence(
					x => x.StreamLongTextAsync(It.IsAny<int>(), It.IsAny<RelativityObjectRef>(), It.IsAny<FieldRef>()))
				.Throws(new InvalidOperationException())
				.ReturnsAsync(this.keplerStream.Object);
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              this.progress).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheSuccessfulLongTextResult(request, result, 1);
			this.VerifyTheStreamLongTextMethodIsCalled(2);
			this.VerifyProgressEventsArePublished(result.Length);
			this.VerifyTheErrorsAreLogged(1);
			this.VerifyTheWarningsAreLogged(1);
			this.VerifyTheStatusNotificationMessagesArePublished(1);
			this.VerifyNoWarningNotificationMessagesArePublished();
			this.VerifyNoErrorNotificationMessagesArePublished();
		}

		[Test]
		public async Task ShouldNotPublishProgressEventsWhenTheProgressIsNull()
		{
			// ARRANGE
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              null).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheSuccessfulLongTextResult(request, result, 0);
			this.VerifyTheStreamLongTextMethodIsCalled(1);
			this.VerifyProgressEventsAreNotPublished();
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		[Test]
		public async Task ShouldNotPublishProgressEventsWhenTheProgressThresholdIsNotReached()
		{
			// ARRANGE
			this.settings.SetupGet(x => x.ExportLongTextLargeFileProgressRateSeconds).Returns(int.MaxValue);
			this.sut = new LongTextStreamService(
				this.factory.Object,
				this.serviceNotification.Object,
				this.settings.Object,
				this.fileSystem,
				this.logger.Object);
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              this.progress).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheSuccessfulLongTextResult(request, result, 0);
			this.VerifyTheStreamLongTextMethodIsCalled(1);
			this.VerifyProgressEventsAreNotPublished();
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		[Test]
		public async Task ShouldRetryNonFatalExceptionsAsync()
		{
			// ARRANGE
			this.objectManager
				.SetupSequence(
					x => x.StreamLongTextAsync(It.IsAny<int>(), It.IsAny<RelativityObjectRef>(), It.IsAny<FieldRef>()))
				.Throws(new InvalidOperationException())
				.Throws(new InvalidOperationException())
				.ReturnsAsync(this.keplerStream.Object);
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT
			LongTextStreamResult result = await this.sut
				                              .SaveLongTextStreamAsync(
					                              request,
					                              this.cancellationTokenSource.Token,
					                              this.progress).ConfigureAwait(false);

			// ASSERT
			this.VerifyTheSuccessfulLongTextResult(request, result, 2);
			this.VerifyTheStreamLongTextMethodIsCalled(3);
			this.VerifyProgressEventsArePublished(result.Length);
			this.VerifyTheErrorsAreLogged(2);
			this.VerifyNoWarningsAreLogged();
			this.VerifyTheStatusNotificationMessagesArePublished(2);
			this.VerifyNoWarningNotificationMessagesArePublished();
			this.VerifyNoErrorNotificationMessagesArePublished();
		}

		[Test]
		public void ShouldThrowWhenCancellationIsRequested()
		{
			// ARRANGE
			this.cancellationTokenSource.Cancel();
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT/ASSERT
			Assert.ThrowsAsync<OperationCanceledException>(
				() => this.sut
					            .SaveLongTextStreamAsync(request, this.cancellationTokenSource.Token, this.progress));
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		[Test]
		public void ShouldThrowFatalExceptions()
		{
			// ARRANGE
			this.objectManager.Setup(
					x => x.StreamLongTextAsync(It.IsAny<int>(), It.IsAny<RelativityObjectRef>(), It.IsAny<FieldRef>()))
				.Throws(new WireProtocolMismatchException());
			LongTextStreamRequest request = this.CreateLongTextStreamRequest();

			// ACT
			Assert.ThrowsAsync<WireProtocolMismatchException>(
				() => this.sut
					            .SaveLongTextStreamAsync(request, this.cancellationTokenSource.Token, this.progress));

			// ASSERT
			Assert.That(this.targetExtractedTextFile, Does.Not.Exist);
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		[TestCaseSource(nameof(InvalidRequestParameters))]
		public void ShouldThrowWhenTheRequestIsInvalid(
			Encoding sourceEncoding,
			int sourceFieldArtifactId,
			int sourceObjectArtifactId,
			Encoding targetEncoding,
			string targetFile,
			int workspaceId)
		{
			// ARRANGE
			LongTextStreamRequest request = new LongTextStreamRequest
				                                {
					                                SourceEncoding = sourceEncoding,
					                                SourceFieldArtifactId = sourceFieldArtifactId,
					                                SourceObjectArtifactId = sourceObjectArtifactId,
					                                TargetEncoding = targetEncoding,
					                                TargetFile = targetFile,
													WorkspaceId = workspaceId
				                                };

			// ACT/ASSERT
			Assert.ThrowsAsync<ArgumentException>(
				() => this.sut
					            .SaveLongTextStreamAsync(request, this.cancellationTokenSource.Token, this.progress));
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		[Test]
		public void ShouldThrowWhenTheRequestIsNull()
		{
			// ARRANGE
			LongTextStreamRequest request = null;

			// ACT/ASSERT
			Assert.ThrowsAsync<ArgumentNullException>(
				() => this.sut
					.SaveLongTextStreamAsync(request, this.cancellationTokenSource.Token, this.progress));
			this.VerifyNoErrorsAreLogged();
			this.VerifyNoWarningsAreLogged();
			this.VerifyNoNotificationMessagesArePublished();
		}

		private static Encoding GetFileEncoding(string file)
		{
			using (var reader = new StreamReader(file, Encoding.Default, true))
			{
				reader.Peek();
				Encoding encoding = reader.CurrentEncoding;
				return encoding;
			}
		}

		private LongTextStreamRequest CreateLongTextStreamRequest()
		{
			return this.CreateLongTextStreamRequest(Encoding.Unicode);
		}

		private LongTextStreamRequest CreateLongTextStreamRequest(Encoding targetEncoding)
		{
			return this.CreateLongTextStreamRequest(targetEncoding, this.targetExtractedTextFile);
		}

		private LongTextStreamRequest CreateLongTextStreamRequest(Encoding targetEncoding, string targetFile)
		{
			return new LongTextStreamRequest
				       {
					       SourceEncoding = Encoding.Unicode,
					       SourceFieldArtifactId = 1,
					       SourceObjectArtifactId = 1,
					       TargetEncoding = targetEncoding,
					       TargetFile = targetFile,
					       WorkspaceId = 1
				       };
		}

		private void VerifyProgressEventsArePublished(long expectedTotalBytesWritten)
		{
			LongTextStreamProgressEventArgs expectedLastProgressArgs = this.publishedProgressArgs.FirstOrDefault(x => x.Completed);
			Assert.That(expectedLastProgressArgs, Is.Not.Null);
			Assert.That(expectedLastProgressArgs.TotalBytesWritten, Is.EqualTo(expectedTotalBytesWritten));
		}

		private void VerifyProgressEventsAreNotPublished()
		{
			Assert.That(this.publishedProgressArgs.Count, Is.Zero);
		}

		private void VerifyTheStreamLongTextMethodIsCalled(int expectedCallCount)
		{
			this.objectManager.Verify(
				x => x.StreamLongTextAsync(It.IsAny<int>(), It.IsAny<RelativityObjectRef>(), It.IsAny<FieldRef>()),
				Times.Exactly(expectedCallCount));
		}

		private void VerifyTheSuccessfulLongTextResult(LongTextStreamRequest request, LongTextStreamResult result, int expectedRetryCount)
		{
			Assert.That(result.Request, Is.Not.Null);
			Assert.That(result.Request, Is.SameAs(request));
			Assert.That(result.Length, Is.EqualTo(new System.IO.FileInfo(this.targetExtractedTextFile).Length));
			Assert.That(result.File, Is.EqualTo(this.targetExtractedTextFile));
			Assert.That(result.FileName, Is.EqualTo(System.IO.Path.GetFileName(this.targetExtractedTextFile)));
			Assert.That(result.File, Does.Exist);
			Assert.That(result.RetryCount, Is.EqualTo(expectedRetryCount));

			// Encoding detection has problems with files that contain characters
			// that are common in both windows-1250 and windows-1252
			// detection results will depend on local system settings
			if (request.TargetEncoding.CodePage == 1252)
			{
				Assert.That(GetFileEncoding(result.File).CodePage, Is.EqualTo(1250).Or.EqualTo(1252));
			}
			else
			{
				Assert.That(GetFileEncoding(result.File), Is.EqualTo(request.TargetEncoding));
			}
		}

		private void VerifyTheNonFatalLongTextResult(LongTextStreamRequest request, LongTextStreamResult result, int expectedRetryCount)
		{
			Assert.That(result.Request, Is.Not.Null);
			Assert.That(result.Request, Is.SameAs(request));
			Assert.That(result.Issue, Is.Not.Null);
			Assert.That(result.Length, Is.Zero);
			Assert.That(result.File, Is.Null.Or.Empty);
			Assert.That(result.FileName, Is.Null.Or.Empty);
			Assert.That(result.RetryCount, Is.EqualTo(expectedRetryCount));
		}

		private void VerifyNoErrorsAreLogged()
		{
			this.VerifyTheErrorsAreLogged(0);
		}

		private void VerifyTheErrorsAreLogged(int expectedCallCount)
		{
			this.logger.Verify(
				x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(expectedCallCount));
		}

		private void VerifyNoWarningsAreLogged()
		{
			this.VerifyTheWarningsAreLogged(0);
		}

		private void VerifyTheWarningsAreLogged(int expectedCallCount)
		{
			this.logger.Verify(
				x => x.LogWarning(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()),
				Times.Exactly(expectedCallCount));
		}

		private void VerifyNoNotificationMessagesArePublished()
		{
			this.VerifyNoStatusNotificationMessagesArePublished();
			this.VerifyNoWarningNotificationMessagesArePublished();
			this.VerifyNoErrorNotificationMessagesArePublished();
		}

		private void VerifyNoStatusNotificationMessagesArePublished()
		{
			this.VerifyTheStatusNotificationMessagesArePublished(0);
		}

		private void VerifyNoWarningNotificationMessagesArePublished()
		{
			this.VerifyTheWarningNotificationMessagesArePublished(0);
		}

		private void VerifyNoErrorNotificationMessagesArePublished()
		{
			this.VerifyTheErrorNotificationMessagesArePublished(0);
		}

		private void VerifyTheErrorNotificationMessagesArePublished(int expectedCallCount)
		{
			this.serviceNotification.Verify(
				x => x.NotifyError(It.IsAny<string>()),
				Times.Exactly(expectedCallCount));
		}

		private void VerifyTheStatusNotificationMessagesArePublished(int expectedCallCount)
		{
			this.serviceNotification.Verify(
				x => x.NotifyStatus(It.IsAny<string>()),
				Times.Exactly(expectedCallCount));
		}

		private void VerifyTheWarningNotificationMessagesArePublished(int expectedCallCount)
		{
			this.serviceNotification.Verify(
				x => x.NotifyWarning(It.IsAny<string>()),
				Times.Exactly(expectedCallCount));
		}
	}
}