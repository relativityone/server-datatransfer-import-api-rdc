// -----------------------------------------------------------------------------------------------------
// <copyright file="IproFullTextLoadFileEntryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.IO;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public abstract class IproFullTextLoadFileEntryTests
	{
		private const string _BATES_NUMBER = "batesNumber";
		private const long _PAGE_OFFSET = 891326;
		private IproFullTextLoadFileEntry _instance;
		private Mock<IRetryableStreamWriter> _writer;
		private Mock<IFullTextLineWriter> _fullTextLineWriter;
		private string _tempFile;

		protected LongTextRepository LongTextRepository { get; private set; }

		protected Mock<IFieldService> FieldService { get; private set; }

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				LogFileFormat = LoadFileType.FileFormat.IPRO_FullText
			};

			this.FieldService = new Mock<IFieldService>();
			this._fullTextLineWriter = new Mock<IFullTextLineWriter>();
			this._writer = new Mock<IRetryableStreamWriter>();

			this.LongTextRepository = new LongTextRepository(new Mock<IFile>().Object, new TestNullLogger());

			LongTextHelper longTextHelper = new LongTextHelper(exportSettings, this.FieldService.Object, this.LongTextRepository);

			this._instance = this.CreateInstance(this.FieldService.Object, longTextHelper, this._fullTextLineWriter.Object);

			this._tempFile = Path.GetTempFileName();
		}

		[TearDown]
		public void TearDown()
		{
			this._instance?.Dispose();
			if (File.Exists(this._tempFile))
			{
				File.Delete(this._tempFile);
			}
		}

		[Test]
		public void ItShouldWriteMultiPageText()
		{
			const string textToWrite = "text to write";

			ObjectExportInfo artifact = new ObjectExportInfo();
			this.PrepareDataSet(artifact, textToWrite);

			TextReader textReader = null;

			this._fullTextLineWriter.Setup(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, It.IsAny<TextReader>(), CancellationToken.None)).Callback(
				(string bn, long po, IRetryableStreamWriter w, TextReader tr, CancellationToken t) => textReader = tr);

			// ACT
			this._instance.WriteFullTextLine(artifact, _BATES_NUMBER, 0, _PAGE_OFFSET, this._writer.Object, CancellationToken.None);
			this._instance.WriteFullTextLine(artifact, _BATES_NUMBER, 1, _PAGE_OFFSET, this._writer.Object, CancellationToken.None);

			// ASSERT
			Assert.That(textReader.ReadToEnd(), Is.EqualTo(textToWrite));
			this._fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, textReader, CancellationToken.None), Times.Exactly(2));
		}

		[Test]
		public void ItShouldWriteText()
		{
			const string textToWrite1 = "text to write 1";
			const string textToWrite2 = "text to write 2";

			ObjectExportInfo artifact1 = new ObjectExportInfo();
			this.PrepareDataSet(artifact1, textToWrite1);

			ObjectExportInfo artifact2 = new ObjectExportInfo();
			this.PrepareDataSet(artifact2, textToWrite2);

			TextReader[] textReaders = new TextReader[2];
			string[] texts = new string[2];
			int i = 0;

			this._fullTextLineWriter.Setup(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, It.IsAny<TextReader>(), CancellationToken.None)).Callback(
				(string bn, long po, IRetryableStreamWriter w, TextReader tr, CancellationToken t) =>
				{
					texts[i] = tr.ReadToEnd();
					textReaders[i++] = tr;
				});

			// ACT
			this._instance.WriteFullTextLine(artifact1, _BATES_NUMBER, 0, _PAGE_OFFSET, this._writer.Object, CancellationToken.None);
			this._instance.WriteFullTextLine(artifact2, _BATES_NUMBER, 0, _PAGE_OFFSET, this._writer.Object, CancellationToken.None);

			// ASSERT
			Assert.That(texts[0], Is.EqualTo(textToWrite1));
			Assert.That(texts[1], Is.EqualTo(textToWrite2));

			this._fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, textReaders[0], CancellationToken.None), Times.Once);
			this._fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, textReaders[1], CancellationToken.None), Times.Once);
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
			const string textToWrite = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;
			const string textInFile = "text in file";

			File.WriteAllText(this._tempFile, textInFile);

			ObjectExportInfo artifact = new ObjectExportInfo();
			this.PrepareDataSetForTooLongText(artifact, textToWrite, this._tempFile);

			TextReader textReader = null;

			this._fullTextLineWriter.Setup(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, It.IsAny<TextReader>(), CancellationToken.None)).Callback(
				(string bn, long po, IRetryableStreamWriter w, TextReader tr, CancellationToken t) => textReader = tr);

			// ACT
			this._instance.WriteFullTextLine(artifact, _BATES_NUMBER, 0, _PAGE_OFFSET, this._writer.Object, CancellationToken.None);

			// ASSERT
			Assert.That(textReader.ReadToEnd(), Is.EqualTo(textInFile));
			this._fullTextLineWriter.Verify(x => x.WriteLine(_BATES_NUMBER, _PAGE_OFFSET, this._writer.Object, textReader, CancellationToken.None), Times.Once);
		}

		protected abstract IproFullTextLoadFileEntry CreateInstance(IFieldService fieldService, LongTextHelper longTextHelper, IFullTextLineWriter fullTextLineWriter);

		protected abstract void PrepareDataSet(ObjectExportInfo artifact, string textToWrite);

		protected abstract void PrepareDataSetForTooLongText(ObjectExportInfo artifact, string textToWrite, string fileLocation);
	}
}