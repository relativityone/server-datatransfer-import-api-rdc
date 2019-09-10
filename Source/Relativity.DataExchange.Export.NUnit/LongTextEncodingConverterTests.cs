// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextEncodingConverterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Reactive.Linq;
	using System.Reactive.Subjects;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextEncodingConverterTests
	{
		private CancellationTokenSource _cancellationTokenSource;

		private LongTextEncodingConverter _instance;

		private LongTextRepository _longTextRepository;

		private Mock<IFileEncodingConverter> _fileEncodingConverter;

		private Mock<IFileTransferProducer> _fileTransferProducerMock;

		private Mock<ILog> _logger;

		private Subject<bool> _fileDownloadCompletedSubject;
		private Subject<string> _fileDownloadSubject;

		[SetUp]
		public void SetUp()
		{
			this._cancellationTokenSource = new CancellationTokenSource();
			this._logger = new Mock<ILog>();
			this._longTextRepository = new LongTextRepository(null, this._logger.Object);
			this._fileEncodingConverter = new Mock<IFileEncodingConverter>();
			this._fileTransferProducerMock = new Mock<IFileTransferProducer>();

			this._instance = new LongTextEncodingConverter(
				this._longTextRepository,
				this._fileEncodingConverter.Object,
				this._logger.Object,
				this._cancellationTokenSource.Token);

			this._fileDownloadCompletedSubject = new Subject<bool>();
			this._fileDownloadSubject = new Subject<string>();

			this._fileTransferProducerMock.SetupGet(item => item.FileDownloaded).Returns(this._fileDownloadSubject.AsObservable());
			this._fileTransferProducerMock.SetupGet(item => item.FileDownloadCompleted).Returns(this._fileDownloadCompletedSubject);

			this._instance.SubscribeForDownloadEvents(this._fileTransferProducerMock.Object);
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
			this._fileDownloadCompletedSubject.OnNext(true);

			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(
					It.IsAny<string>(),
					It.IsAny<Encoding>(),
					It.IsAny<Encoding>(),
					this._cancellationTokenSource.Token),
				Times.Never);
			Assert.That(this._instance.Count, Is.Zero);
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
			this._fileDownloadCompletedSubject.OnNext(true);

			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(
					It.IsAny<string>(),
					It.IsAny<Encoding>(),
					It.IsAny<Encoding>(),
					this._cancellationTokenSource.Token),
				Times.Never);
			Assert.That(this._instance.Count, Is.Zero);
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
			this._fileDownloadCompletedSubject.OnNext(true);

			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
			Assert.That(longText.SourceEncoding, Is.EqualTo(longText.DestinationEncoding));
			Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public void ItShouldThrowWhenConvertingNewFileAfterMarkingQueueAsCompleted()
		{
			// ARRANGE
			string fileName = "fileName";

			// ACT

			// Mark a conversion task as completed
			this._fileDownloadCompletedSubject.OnNext(true);

			// ASSERT
			Assert.Throws<InvalidOperationException>(() => this._fileDownloadSubject.OnNext(fileName));
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

			this._fileDownloadCompletedSubject.OnNext(true);
			await this._instance.WaitForConversionCompletion().ConfigureAwait(false);

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
			Assert.That(this._instance.Count, Is.Zero);
		}
	}
}