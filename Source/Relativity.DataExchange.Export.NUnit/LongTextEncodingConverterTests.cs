// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextEncodingConverterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextEncodingConverterTests
	{
		private CancellationTokenSource _cancellationTokenSource;

		private LongTextEncodingConverter _instance;

		private LongTextRepository _longTextRepository;

		private Mock<IStatus> _status;

		private Mock<IFileEncodingConverter> _fileEncodingConverter;

		private Mock<IFileTransferProducer> _fileTransferProducerMock;

		private Subject<string> _fileDownloadSubject;

		[SetUp]
		public void SetUp()
		{
			this._cancellationTokenSource = new CancellationTokenSource();
			var testLogger = new TestNullLogger();
			this._longTextRepository = new LongTextRepository(null, testLogger);
			this._fileEncodingConverter = new Mock<IFileEncodingConverter>();
			this._fileTransferProducerMock = new Mock<IFileTransferProducer>();

			this._status = new Mock<IStatus>();

			this._instance = new LongTextEncodingConverter(
				this._longTextRepository,
				this._fileEncodingConverter.Object,
				this._status.Object,
				testLogger);

			this._fileDownloadSubject = new Subject<string>();

			this._fileTransferProducerMock.SetupGet(item => item.FileDownloaded).Returns(this._fileDownloadSubject.AsObservable());

			this._instance.SubscribeForDownloadEvents(this._fileTransferProducerMock.Object, this._cancellationTokenSource.Token);
		}

		[Test]
		public async Task ItShouldNotConvertFileWhenNotListening()
		{
			// ARRANGE
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(
					It.IsAny<string>(),
					It.IsAny<Encoding>(),
					It.IsAny<Encoding>(),
					this._cancellationTokenSource.Token),
				Times.Never);

			// Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public async Task ItShouldNotConvertFileWhenConversionIsNotRequired()
		{
			// ARRANGE
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.Unicode;

			// ACT
			this._fileDownloadSubject.OnNext(fileName);

			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(
					It.IsAny<string>(),
					It.IsAny<Encoding>(),
					It.IsAny<Encoding>(),
					this._cancellationTokenSource.Token),
				Times.Never);

			// Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public async Task ItShouldConvertFile()
		{
			// ARRANGE
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT
			this._fileDownloadSubject.OnNext(fileName);

			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
			Assert.That(longText.SourceEncoding, Is.EqualTo(longText.DestinationEncoding));

			// Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public void ItShouldNeverConvertWhenJobWasCancelled()
		{
			// ARRANGE
			string fileName = "fileName";

			// Mark entire job as cancelled
			this._cancellationTokenSource.Cancel();
			this._fileDownloadSubject.OnNext(fileName);

			// ASSERT
			_fileEncodingConverter.Verify(
				x => x.Convert(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<Encoding>(), It.IsAny<CancellationToken>()), Times.Never);
		}

		[Test]
		public async Task ItShouldWaitForConversionToComplete()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;
			this._fileEncodingConverter.Setup(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token));

			// ACT
			this._fileDownloadSubject.OnNext(fileName);

			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
		}
	}
}