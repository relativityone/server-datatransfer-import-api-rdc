using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Repository
{
	[TestFixture]
	public class LongTextPrecedenceBuilderTests
	{
		private LongTextPrecedenceBuilder _instance;

		private ViewFieldInfo _textPrecedence;
		private ExportFile _exportSettings;

		private Mock<IFilePathProvider> _filePathProvider;
		private Mock<IFieldService> _fieldService;
		private Mock<IFileNameProvider> _fileNameProvider;
		private Mock<IExportFileValidator> _exportFileValidator;
		private Mock<IMetadataProcessingStatistics> _metadataProcessingStatistics;

		private string _fileToDelete;

		[SetUp]
		public void SetUp()
		{
			_textPrecedence = new QueryFieldFactory().GetArtifactIdField();
			_exportSettings = new ExportFile(1)
			{
				SelectedTextFields = new[] {_textPrecedence},
				TextFileEncoding = Encoding.Default
			};

			_filePathProvider = new Mock<IFilePathProvider>();
			_fieldService = new Mock<IFieldService>();
			_fileNameProvider = new Mock<IFileNameProvider>();
			_exportFileValidator = new Mock<IExportFileValidator>();
			_metadataProcessingStatistics = new Mock<IMetadataProcessingStatistics>();

			_fileToDelete = Path.GetTempFileName();

			_fieldService.Setup(x => x.GetOrdinalIndex(Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)).Returns(0);
			_fieldService.Setup(x => x.GetOrdinalIndex(Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)).Returns(1);

			LongTextHelper longTextHelper = new LongTextHelper(_exportSettings, _fieldService.Object, new LongTextRepository(null, new NullLogger()));

			_instance = new LongTextPrecedenceBuilder(_exportSettings, _filePathProvider.Object, _fieldService.Object, longTextHelper, _fileNameProvider.Object, new NullLogger(),
				_exportFileValidator.Object, _metadataProcessingStatistics.Object);
		}

		[TearDown]
		public void TearDown()
		{
			if (!string.IsNullOrWhiteSpace(_fileToDelete) && File.Exists(_fileToDelete))
			{
				File.Delete(_fileToDelete);
			}
		}

		[Test]
		public void ItShouldHandleLongTextToLoadFile()
		{
			const string notTooLongText = "not too long text";

			_exportSettings.ExportFullTextAsFile = false;
			
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					_textPrecedence.FieldArtifactId,
					notTooLongText
				}
			};

			//ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
			Assert.That(actualResult[0].GetLongText().ReadToEnd(), Is.EqualTo(notTooLongText));
		}

		[Test]
		public void ItShouldHandleLongTextToFile()
		{
			const string notTooLongText = "not too long text";

			_exportSettings.ExportFullTextAsFile = true;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					_textPrecedence.FieldArtifactId,
					notTooLongText
				}
			};
			
			_exportFileValidator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			_filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>())).Returns(_fileToDelete);

			//ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
			using (var reader = actualResult[0].GetLongText())
			{
				Assert.That(reader.ReadToEnd(), Is.EqualTo(notTooLongText));
			}
		}

		[Test]
		public void ItShouldHandleTooLongTextToLoadFile()
		{
			const string tooLongText = Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			_exportSettings.ExportFullTextAsFile = false;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					_textPrecedence.FieldArtifactId,
					tooLongText
				}
			};

			//ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Not.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.True);
		}

		[Test]
		public void ItShouldHandleTooLongTextToFile()
		{
			const string tooLongText = Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			_exportSettings.ExportFullTextAsFile = true;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					_textPrecedence.FieldArtifactId,
					tooLongText
				}
			};

			_exportFileValidator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
			_filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>())).Returns(_fileToDelete);

			//ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Not.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
		}

		[Test]
		public void ItShouldHandleTooLongTextToFile_FileAlreadyExists()
		{
			const string tooLongText = Relativity.Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			_exportSettings.ExportFullTextAsFile = true;

			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[]
				{
					_textPrecedence.FieldArtifactId,
					tooLongText
				}
			};

			_exportFileValidator.Setup(x => x.CanExport(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
			_filePathProvider.Setup(x => x.GetPathForFile(It.IsAny<string>())).Returns(_fileToDelete);

			//ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

			//ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].RequireDeletion, Is.False);
		}
	}
}