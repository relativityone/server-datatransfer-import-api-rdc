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
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	[TestFixture]
	public class LongTextEncodingConverterTests
	{
		private LongTextEncodingConverter _instance;

		private LongTextRepository _longTextRepository;

		private Mock<IFileEncodingConverter> _fileEncodingConverter;
		private Mock<ITapiBridge> _tapiBridge;

		[SetUp]
		public void SetUp()
		{
			this._longTextRepository = new LongTextRepository(null, new NullLogger());
			this._fileEncodingConverter = new Mock<IFileEncodingConverter>();
			this._tapiBridge = new Mock<ITapiBridge>();

			this._instance = new LongTextEncodingConverter(this._longTextRepository, this._fileEncodingConverter.Object, new NullLogger(), CancellationToken.None);
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

			bool waited = false;

			this._fileEncodingConverter.Setup(x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None)).Callback(() =>
			{
				Thread.Sleep(2000);
				waited = true;
			});

			// ACT
			this._instance.StartListening(this._tapiBridge.Object);

			this._tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, true, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			this._instance.StopListening(this._tapiBridge.Object);

			this._instance.WaitForConversionCompletion();

			// ASSERT
			Assert.That(waited, Is.True);
		}
	}
}