// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextEncodingConverterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
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
			_longTextRepository = new LongTextRepository(null, new NullLogger());
			_fileEncodingConverter = new Mock<IFileEncodingConverter>();
			_tapiBridge = new Mock<ITapiBridge>();

			_instance = new LongTextEncodingConverter(_longTextRepository, _fileEncodingConverter.Object, new NullLogger(), CancellationToken.None);
		}

		[Test]
		public void ItShouldNotConvertFileWhenNotListening()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, _longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.ASCII;

			// ACT
			_instance.StartListening(_tapiBridge.Object);
			_instance.StopListening(_tapiBridge.Object);

			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, TransferPathStatus.Successful, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			_instance.WaitForConversionCompletion();

			// ASSERT
			_fileEncodingConverter.Verify(x => x.Convert(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<Encoding>(), CancellationToken.None), Times.Never);
		}

		[Test]
		public void ItShouldNotConvertFileWhenConversionIsNotRequired()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, _longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.Unicode;

			// ACT
			_instance.StartListening(_tapiBridge.Object);

			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, TransferPathStatus.Successful, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			_instance.StopListening(_tapiBridge.Object);

			_instance.WaitForConversionCompletion();

			// ASSERT
			_fileEncodingConverter.Verify(x => x.Convert(It.IsAny<string>(), It.IsAny<Encoding>(), It.IsAny<Encoding>(), CancellationToken.None), Times.Never);
		}

		[Test]
		public void ItShouldConvertFile()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, _longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			// ACT
			_instance.StartListening(_tapiBridge.Object);

			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, TransferPathStatus.Successful, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			_instance.StopListening(_tapiBridge.Object);

			_instance.WaitForConversionCompletion();

			// ASSERT
			_fileEncodingConverter.Verify(x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None), Times.Once);
			Assert.That(longText.SourceEncoding, Is.EqualTo(longText.DestinationEncoding));
		}

		[Test]
		public void ItShouldWaitForConversionToComplete()
		{
			const string fileName = "fileName";

			LongText longText = ModelFactory.GetLongTextWithLocationAndEncoding(1, _longTextRepository, fileName, Encoding.Unicode);
			longText.SourceEncoding = Encoding.UTF8;

			bool waited = false;

			_fileEncodingConverter.Setup(x => x.Convert(fileName, Encoding.UTF8, Encoding.Unicode, CancellationToken.None)).Callback(() =>
			{
				Thread.Sleep(2000);
				waited = true;
			});

			// ACT
			_instance.StartListening(_tapiBridge.Object);

			_tapiBridge.Raise(x => x.TapiProgress += null, new TapiProgressEventArgs(fileName, true, TransferPathStatus.Successful, 1, 1, DateTime.MinValue, DateTime.MaxValue));

			_instance.StopListening(_tapiBridge.Object);

			_instance.WaitForConversionCompletion();

			// ASSERT
			Assert.That(waited, Is.True);
		}
	}
}