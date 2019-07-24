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
		private LongTextEncodingConverter _instance;

		private LongTextRepository _longTextRepository;

		private Mock<IFileEncodingConverter> _fileEncodingConverter;
		private Mock<ITapiBridge> _tapiBridge;
		private Mock<ILog> _logger;

		[SetUp]
		public void SetUp()
		{
			this._logger = new Mock<ILog>();
			this._longTextRepository = new LongTextRepository(null, this._logger.Object);
			this._fileEncodingConverter = new Mock<IFileEncodingConverter>();
			this._tapiBridge = new Mock<ITapiBridge>();
			this._instance = new LongTextEncodingConverter(
				this._longTextRepository,
				this._fileEncodingConverter.Object,
				this._logger.Object,
				CancellationToken.None);
		}

		[Test]
		public void ItShouldNotConvertFileWhenNotListening()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, this._longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			this._instance.StartListening(this._tapiBridge.Object);
			this._instance.StopListening(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(x => x.Convert(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<Encoding>(), CancellationToken.None), Times.Never);
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void ItShouldNotConvertFileWhenTheFileIsNotSuccessfullyTransferred(bool completed)
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, this._longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			this._instance.StartListening(this._tapiBridge.Object);
			this._instance.StopListening(this._tapiBridge.Object);
			const bool Successful = false;
			this._tapiBridge.Raise(
				x => x.TapiProgress += null,
				new TapiProgressEventArgs(fileName, completed, Successful, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<Encoding>(), CancellationToken.None),
				Times.Never);
		}

		[Test]
		public void ItShouldNotConvertFileWhenConversionIsNotRequired()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, this._longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.Unicode;

			// ACT
			this._instance.StartListening(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.StopListening(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(x => x.Convert(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<Encoding>(), CancellationToken.None), Times.Never);
		}

		[Test]
		public void ItShouldConvertFile()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, this._longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT
			this._instance.StartListening(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.StopListening(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None), Times.Once);
			Assert.That(longText.SourceEncoding, Is.EqualTo(longText.DestinationEncoding));
		}

		[Test]
		public void ItShouldWaitForConversionToComplete()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, this._longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;
			this._fileEncodingConverter.Setup(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None));

			// ACT
			this._instance.StartListening(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.StopListening(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

			// ASSERT
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None),
				Times.Once);
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
			this._instance.StartListening(this._tapiBridge.Object);
			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));
			this._instance.StopListening(this._tapiBridge.Object);

			// ASSERT
			Assert.Throws<ArgumentException>(() => this._instance.WaitForConversionCompletion());
			this._fileEncodingConverter.Verify(
				x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None),
				Times.Never);
			this._logger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
		}
	}
}