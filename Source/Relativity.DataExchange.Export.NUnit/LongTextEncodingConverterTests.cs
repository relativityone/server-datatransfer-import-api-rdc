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

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	[TestFixture]
	public class LongTextEncodingConverterTests
	{
		private CancellationTokenSource _cancellationTokenSource;

		private LongTextEncodingConverter _instance;

		private LongTextRepository _longTextRepository;

		private Mock<IFileEncodingConverter> _fileEncodingConverter;

		private Mock<ITapiBridge> _tapiBridge;

		private Mock<ILog> _logger;

		[SetUp]
		public void SetUp()
		{
			this._cancellationTokenSource = new CancellationTokenSource();
			this._logger = new Mock<ILog>();
			this._longTextRepository = new LongTextRepository(null, this._logger.Object);
			this._fileEncodingConverter = new Mock<IFileEncodingConverter>();
			this._tapiBridge = new Mock<ITapiBridge>();
			this._instance = new LongTextEncodingConverter(
				this._longTextRepository,
				this._fileEncodingConverter.Object,
				this._logger.Object,
				this._cancellationTokenSource.Token);
		}

		[Test]
		public void ItShouldNotConvertFileWhenNotListening()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);
			this._instance.Unsubscribe(this._tapiBridge.Object);

			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.WaitForConversionCompletion();

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
		public void ItShouldNotConvertFileWhenTheFileIsNotSuccessfullyTransferred(bool completed)
		{
			const bool Successful = false;
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);
			this._instance.Unsubscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, completed, Successful, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.WaitForConversionCompletion();

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
		public void ItShouldNotConvertFileWhenConversionIsNotRequired()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.Unicode;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);

			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.Unsubscribe(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

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
		public void ItShouldConvertFile()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName,
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);

			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.Unsubscribe(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
			Assert.That(longText.SourceEncoding, Is.EqualTo(longText.DestinationEncoding));
			Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public void ItShouldThrowWhenProgressIsRaisedAfterTheQueueIsMarkedComplete()
		{
			// ARRANGE
			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				"fileName",
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT - This simulates the scenario where events are raised despite having been unsubscribed.
			this._instance.Subscribe(this._tapiBridge.Object);
			this._instance.MarkQueueComplete();

			// ASSERT
			InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
				() =>
					{
						this._tapiBridge.Raise(
							x => x.TapiProgress += null,
							new TapiProgressEventArgs(
								longText.Location,
								true,
								true,
								1,
								1,
								DateTime.MinValue,
								DateTime.MaxValue));
					});
			this._logger.Verify(x => x.LogError(exception, It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}

		[Test]
		public void ItShouldWaitForConversionToComplete()
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
			this._instance.Subscribe(this._tapiBridge.Object);

			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.Unsubscribe(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Once);
			Assert.That(this._instance.Count, Is.Zero);
		}

		[Test]
		public void ItShouldThrowWhenTheFileDoesNotExistInTheRepository()
		{
			// Simulate an inability to find the LongText object within the repository.
			const string fileName = "fileName";
			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(
				1,
				this._longTextRepository,
				fileName + "-file-not-found",
				Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT
			this._instance.Subscribe(this._tapiBridge.Object);
			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));
			this._instance.Unsubscribe(this._tapiBridge.Object);

			// ASSERT
			Assert.Throws<ArgumentException>(() => this._instance.WaitForConversionCompletion());
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, this._cancellationTokenSource.Token),
				Times.Never);
			this._logger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}

		[Test]
		public void ItShouldDisposeTheBlockingCollection()
		{
			// ARRANGE
			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs("filename1", true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			// ACT
			this._instance.Dispose();

			// ASSERT
			Assert.Throws<ObjectDisposedException>(() => this._instance.MarkQueueComplete());
		}
	}
}