using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Constants = Relativity.Constants;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images.Lines
{
	[TestFixture]
	public abstract class IproFullTextLoadFileEntryTests
	{
		private IproFullTextLoadFileEntry _instance;

		private Mock<IRetryableStreamWriter> _writer;
		private Mock<IFullTextLineWriter> _fullTextLineWriter;

		private string _tempFile;

		private const string _BATES_NUMBER = "batesNumber";
		private const long _PAGE_OFFSET = 891326;

		protected LongTextRepository LongTextRepository { get; private set; }
		protected Mock<IFieldService> FieldService { get; private set; }

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				LogFileFormat = LoadFileType.FileFormat.IPRO_FullText
			};

			FieldService = new Mock<IFieldService>();
			_fullTextLineWriter = new Mock<IFullTextLineWriter>();
			_writer = new Mock<IRetryableStreamWriter>();

			LongTextRepository = new LongTextRepository(new Mock<IFileHelper>().Object, new NullLogger());

			LongTextHelper longTextHelper = new LongTextHelper(exportSettings, FieldService.Object, LongTextRepository);

			_instance = CreateInstance(FieldService.Object, longTextHelper, _fullTextLineWriter.Object);

			_tempFile = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			_instance?.Dispose();
			if (File.Exists(_tempFile))
			{
				File.Delete(_tempFile);
			}
		}

		[Test]
		public void ItShouldWriteMultiPageText()
		{
			const string textToWrite = "text to write";

			ObjectExportInfo artifact = new ObjectExportInfo();
			PrepareDataSet(artifact, textToWrite);

			TextReader textReader = null;

			_fullTextLineWriter.Setup(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, It.IsAny<TextReader>(), CancellationToken.None)).Callback(
				(string bn, long po, IRetryableStreamWriter w, TextReader tr, CancellationToken t) => textReader = tr);

			//ACT
			_instance.WriteFullTextLine(artifact, _BATES_NUMBER, 0, _PAGE_OFFSET, _writer.Object, CancellationToken.None);
			_instance.WriteFullTextLine(artifact, _BATES_NUMBER, 1, _PAGE_OFFSET, _writer.Object, CancellationToken.None);

			//ASSERT
			Assert.That(textReader.ReadToEnd(), Is.EqualTo(textToWrite));
			_fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, textReader, CancellationToken.None), Times.Exactly(2));
		}

		[Test]
		public void ItShouldWriteText()
		{
			const string textToWrite1 = "text to write 1";
			const string textToWrite2 = "text to write 2";

			ObjectExportInfo artifact1 = new ObjectExportInfo();
			PrepareDataSet(artifact1, textToWrite1);

			ObjectExportInfo artifact2 = new ObjectExportInfo();
			PrepareDataSet(artifact2, textToWrite2);

			TextReader[] textReaders = new TextReader[2];
			string[] texts = new string[2];
			int i = 0;

			_fullTextLineWriter.Setup(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, It.IsAny<TextReader>(), CancellationToken.None)).Callback(
				(string bn, long po, IRetryableStreamWriter w, TextReader tr, CancellationToken t) =>
				{
					texts[i] = tr.ReadToEnd();
					textReaders[i++] = tr;
				});

			//ACT
			_instance.WriteFullTextLine(artifact1, _BATES_NUMBER, 0, _PAGE_OFFSET, _writer.Object, CancellationToken.None);
			_instance.WriteFullTextLine(artifact2, _BATES_NUMBER, 0, _PAGE_OFFSET, _writer.Object, CancellationToken.None);

			//ASSERT
			Assert.That(texts[0], Is.EqualTo(textToWrite1));
			Assert.That(texts[1], Is.EqualTo(textToWrite2));

			_fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, textReaders[0], CancellationToken.None), Times.Once);
			_fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, textReaders[1], CancellationToken.None), Times.Once);
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
			const string textToWrite = Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;
			const string textInFile = "text in file";

			File.WriteAllText(_tempFile, textInFile);

			ObjectExportInfo artifact = new ObjectExportInfo();
			PrepareDataSetForTooLongText(artifact, textToWrite, _tempFile);

			TextReader textReader = null;

			_fullTextLineWriter.Setup(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, It.IsAny<TextReader>(), CancellationToken.None)).Callback(
				(string bn, long po, IRetryableStreamWriter w, TextReader tr, CancellationToken t) => textReader = tr);

			//ACT
			_instance.WriteFullTextLine(artifact, _BATES_NUMBER, 0, _PAGE_OFFSET, _writer.Object, CancellationToken.None);

			//ASSERT
			Assert.That(textReader.ReadToEnd(), Is.EqualTo(textInFile));
			_fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, _writer.Object, textReader, CancellationToken.None), Times.Once);
		}

		protected abstract IproFullTextLoadFileEntry CreateInstance(IFieldService fieldService, LongTextHelper longTextHelper, IFullTextLineWriter fullTextLineWriter);
		protected abstract void PrepareDataSet(ObjectExportInfo artifact, string textToWrite);
		protected abstract void PrepareDataSetForTooLongText(ObjectExportInfo artifact, string textToWrite, string fileLocation);
	}
}