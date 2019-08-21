// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextEncodingConverterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Moq;

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

		private Mock<ILog> _logger;

		[SetUp]
		public void SetUp()
		{
			this._cancellationTokenSource = new CancellationTokenSource();
			this._logger = new Mock<ILog>();
			this._longTextRepository = new LongTextRepository(null, this._logger.Object);
			this._fileEncodingConverter = new Mock<IFileEncodingConverter>();
			this._instance = new LongTextEncodingConverter(
				this._longTextRepository,
				this._fileEncodingConverter.Object,
				this._logger.Object,
				this._cancellationTokenSource.Token);
		}

		[Test]
		public async Task ItShouldNotConvertFileWhenNotListening()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			this._instance.NotifyStartConversion();
			this._instance.NotifyStopConversion();

			await this._instance.WaitForConversionCompletion();

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
		[TestCase(false)]
		[TestCase(true)]
		public async Task ItShouldNotConvertFileWhenTheFileIsNotSuccessfullyTransferred(bool completed)
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			this._instance.NotifyStartConversion();
			this._instance.NotifyStopConversion();

			await this._instance.WaitForConversionCompletion();

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
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.Unicode;

			// ACT
			this._instance.NotifyStartConversion();

			this._instance.NotifyStopConversion();

			await this._instance.WaitForConversionCompletion();

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
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT
			this._instance.NotifyStartConversion();
			this._instance.AddForConversion(fileName);
			this._instance.NotifyStopConversion();

			await this._instance.WaitForConversionCompletion();

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

			// ACT - This simulates the scenario where events are raised despite having been unsubscribed.
			this._instance.NotifyStartConversion();
			this._instance.NotifyStopConversion();

			Assert.Throws<InvalidOperationException>(() => this._instance.AddForConversion(fileName));
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
			this._instance.NotifyStartConversion();

			this._instance.AddForConversion(fileName);

			this._instance.NotifyStopConversion();

			await this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
			Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public void ItShouldDisposeTheBlockingCollection()
		{
			// ACT
			this._instance.Dispose();

			// ASSERT
			Assert.Throws<ObjectDisposedException>(() => this._instance.NotifyStopConversion());
		}
	}
}